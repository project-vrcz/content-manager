using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Files;

namespace VRChatContentManager.Core.Services.VRChatApi.S3;

public sealed class ConcurrentMultipartUploader(
    VRChatApiClient apiClient,
    HttpClient awsClient,
    ILogger<ConcurrentMultipartUploader> logger,
    Stream fileStream,
    string fileId,
    int fileVersion,
    VRChatApiFileType fileType,
    CancellationToken cancellationToken)
{
    public event EventHandler<double>? ProgressChanged;

    // The size of each part uploaded to S3. 50MB is a reasonable default.
    private const long ChunkSize = 50 * 1024 * 1024;

    // Controls how many chunks are uploaded in parallel.
    private const int MaxConcurrentUploads = 3;

    private readonly ConcurrentDictionary<int, string> _eTags = new();
    private long _totalBytesUploaded;

    public async Task<string[]> UploadAsync()
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation(
            "Starting concurrent upload for file {FileId} version {FileVersion} with chunk size {ChunkSize}MB and concurrency {MaxConcurrency}",
            fileId, fileVersion, ChunkSize / 1024 / 1024, MaxConcurrentUploads);

        using var concurrencySemaphore = new SemaphoreSlim(MaxConcurrentUploads);
        var uploadTasks = new List<Task>();
        var partNumber = 0;

        try
        {
            while (fileStream.Position < fileStream.Length)
            {
                cancellationToken.ThrowIfCancellationRequested();

                partNumber++;
                var currentPartNumber = partNumber;

                var buffer = new byte[ChunkSize];
                var bytesRead = await fileStream.ReadAsync(buffer, cancellationToken);

                // Wait for a free slot to begin the next upload.
                await concurrencySemaphore.WaitAsync(cancellationToken);

                // Start the upload task for the current chunk.
                var uploadTask = Task.Run(async () =>
                {
                    try
                    {
                        await UploadChunkAsync(currentPartNumber, buffer, bytesRead, cancellationToken);
                    }
                    finally
                    {
                        // Release the semaphore slot once the upload is complete or has failed.
                        // ReSharper disable once AccessToDisposedClosure
                        concurrencySemaphore.Release();
                    }
                }, cancellationToken);

                uploadTasks.Add(uploadTask);
            }

            // Wait for all initiated upload tasks to complete.
            await Task.WhenAll(uploadTasks);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Upload for file {FileId} version {FileVersion} was canceled", fileId,
                fileVersion);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during the upload of file {FileId} version {FileVersion}", fileId,
                fileVersion);
            // Rethrow to allow the caller to handle the failure.
            throw;
        }

        logger.LogInformation("Successfully uploaded all parts for file {FileId} version {FileVersion}", fileId,
            fileVersion);

        // Return the ETag for each part, ordered by part number.
        return _eTags.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();
    }

    private async Task UploadChunkAsync(int partNumber, byte[] buffer, int bytesRead, CancellationToken ct)
    {
        logger.LogInformation(
            "Creating upload chunk {PartNumber} (Size: {Size:F2} MiB) for file {FileId}",
            partNumber, (double)bytesRead / 1024 / 1024, fileId);

        // 1. Get the pre-signed URL for this part from the VRChat API.
        var uploadUrl = await apiClient.GetFilePartUploadUrlAsync(fileId, fileVersion, partNumber, fileType, ct);

        // 2. Upload the data to the provided URL.
        using var stream = new MemoryStream(buffer, 0, bytesRead);
        using var progressStream = new ProgressStreamContent(stream, OnUploadProgress);

        var response = await awsClient.PutAsync(uploadUrl, progressStream, ct);
        response.EnsureSuccessStatusCode();

        // 3. Extract the ETag from the response headers. This is required by S3 to complete the multipart upload.
        var eTag = response.Headers.ETag?.Tag.Trim('\"', '\'');
        if (string.IsNullOrEmpty(eTag))
        {
            throw new InvalidOperationException($"S3 did not return an ETag for part {partNumber} of file {fileId}.");
        }

        _eTags[partNumber] = eTag;
        logger.LogInformation("Completed upload for chunk {PartNumber} for file {FileId}", partNumber, fileId);
    }

    private void OnUploadProgress(long uploadedBytes)
    {
        Interlocked.Add(ref _totalBytesUploaded, uploadedBytes);

        if (fileStream.Length == 0) return;

        var progress = (double)_totalBytesUploaded / fileStream.Length;
        ProgressChanged?.Invoke(this, progress);
    }
}

public sealed class ConcurrentMultipartUploaderFactory(ILogger<ConcurrentMultipartUploader> logger)
{
    public ConcurrentMultipartUploader Create(Stream fileStream, string fileId, int fileVersion,
        VRChatApiFileType fileType, VRChatApiClient apiClient, HttpClient awsClient,
        CancellationToken cancellationToken = default)
    {
        return new ConcurrentMultipartUploader(apiClient, awsClient, logger, fileStream, fileId, fileVersion, fileType,
            cancellationToken);
    }
}