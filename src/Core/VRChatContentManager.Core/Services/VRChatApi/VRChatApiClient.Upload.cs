using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using VRChatContentManager.Core.Models;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Files;
using VRChatContentManager.Core.Utils;

namespace VRChatContentManager.Core.Services.VRChatApi;

public partial class VRChatApiClient
{
    public async ValueTask<VRChatApiFileVersion> CreateAndUploadFileVersionAsync(
        Stream fileStream, string fileId, string contentType,
        HttpClient awsClient, string userFileType, Action<PublishTaskProgressEventArg>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var currentAssetFile = await GetFileAsync(fileId, cancellationToken);

        // Step 1. Cleanup any incomplete file versions.
        progressCallback?.Invoke(new PublishTaskProgressEventArg("Cleanup all incomplete file versions...", null,
            ContentPublishTaskStatus.InProgress));

        if (!await CleanupIncompleteFileVersionsAsync(currentAssetFile, this, cancellationToken))
        {
            currentAssetFile = await GetFileAsync(fileId, cancellationToken);
        }

        // Step 2.Caulate bundle file md5 and check is same file exists.
        progressCallback?.Invoke(new PublishTaskProgressEventArg($"Calculating MD5 for {userFileType} file...", null,
            ContentPublishTaskStatus.InProgress));

        var fileMd5 = await VRChatApiFlieUtils.GetMd5FromStreamForVRChatAsync(fileStream, cancellationToken);
        var fileLength = fileStream.Length;

        var existingFileVersion = currentAssetFile.Versions
            .Where(version => version.File is { } file && file.Md5 == fileMd5)
            .ToArray();

        if (existingFileVersion.Length > 0)
        {
            logger.LogInformation("File with same MD5 already exists for file {FileId}", fileId);
            if (existingFileVersion.FirstOrDefault(version => version.Status == "complete") is { } completeVersion)
            {
                logger.LogInformation("Existing file version {Version} is already complete for file {FileId}",
                    completeVersion.Version, fileId);
                // TODO: What will happen? I don't know.
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        // Step 3. Caulate file signature
        progressCallback?.Invoke(new PublishTaskProgressEventArg($"Calculating (Blake2b) Signature for {userFileType} file...",
            null, ContentPublishTaskStatus.InProgress));

        var signatureStream =
            new MemoryStream(
                await VRChatApiFlieUtils.GetSignatureFromStreamForVRChatAsync(fileStream, cancellationToken));
        var signatureMd5 = await VRChatApiFlieUtils.GetMd5FromStreamForVRChatAsync(signatureStream, cancellationToken);
        var signatureLength = signatureStream.Length;

        logger.LogInformation("Creating new file version for file {FileId}", fileId);

        // Step 4. Create new file version
        progressCallback?.Invoke(new PublishTaskProgressEventArg("Creating new file version...", null,
            ContentPublishTaskStatus.InProgress));

        var fileVersion =
            await CreateFileVersionAsync(fileId, fileMd5, fileLength, signatureMd5, signatureLength, cancellationToken);

        if (fileVersion.File is null || fileVersion.Signature is null)
            throw new UnexpectedApiBehaviourException(
                "Api did not return file or signature info for created file version.");

        // Step 5. Upload bundle file and signature to aws s3
        logger.LogInformation("Uploading file version {Version} for file {FileId}", fileVersion.Version, fileId);
        progressCallback?.Invoke(new PublishTaskProgressEventArg($"Preparing for Upload {userFileType} file...", null,
            ContentPublishTaskStatus.InProgress));

        await UploadFileVersionAsync(fileStream, fileId, fileVersion.Version, fileMd5,
            fileVersion.File.Category == "simple", VRChatApiFileType.File, contentType, awsClient,
            progress => progressCallback?.Invoke(new PublishTaskProgressEventArg($"Uploading {userFileType} file...",
                progress, ContentPublishTaskStatus.InProgress)), cancellationToken);

        logger.LogInformation("Uploading signature for {FileId}", fileId);
        progressCallback?.Invoke(new PublishTaskProgressEventArg("Preparing for Upload signature...", null,
            ContentPublishTaskStatus.InProgress));

        await UploadFileVersionAsync(signatureStream, fileId, fileVersion.Version, signatureMd5,
            fileVersion.Signature.Category == "simple", VRChatApiFileType.Signature, contentType, awsClient,
            progress => progressCallback?.Invoke(new PublishTaskProgressEventArg("Uploading signature...", progress,
                ContentPublishTaskStatus.InProgress)), cancellationToken);

        // Step 6. Wait for server to process the uploaded file version
        logger.LogInformation("Waiting for server processing of file version {Version} for file {FileId}",
            fileVersion.Version, fileId);
        progressCallback?.Invoke(new PublishTaskProgressEventArg("Waiting for server processing (for 3s)...", null,
            ContentPublishTaskStatus.InProgress));

        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        logger.LogInformation("Fetching completed file version {Version} for file {FileId}", fileVersion.Version,
            fileId);
        progressCallback?.Invoke(new PublishTaskProgressEventArg("Fetching new file version detail...", null,
            ContentPublishTaskStatus.InProgress));

        var completedFile = await GetFileAsync(fileId, cancellationToken);
        return completedFile.Versions.FirstOrDefault(ver => ver.Version == fileVersion.Version) ??
               throw new UnexpectedApiBehaviourException(
                   "Api did not return the created file version after upload complete.");
    }

    private async ValueTask UploadFileVersionAsync(Stream fileStream, string fileId, int version, string md5,
        bool isSimpleUpload, VRChatApiFileType fileType, string? contentType,
        HttpClient awsClient, Action<double?>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        progressCallback?.Invoke(null);
        if (isSimpleUpload)
        {
            var simpleUploadUrl = await GetSimpleUploadUrlAsync(fileId, version, fileType, cancellationToken);
            await PutFileAsync(simpleUploadUrl, fileStream, awsClient, md5, isSimpleUpload, contentType, cancellationToken);
            await CompleteSimpleFileUploadAsync(fileId, version, fileType, cancellationToken);

            progressCallback?.Invoke(1);

            return;
        }

        var uploader =
            concurrentMultipartUploaderFactory.Create(fileStream, fileId, version, fileType, this, awsClient,
                cancellationToken);
        uploader.ProgressChanged += (_, progress) => progressCallback?.Invoke(progress);

        var eTags = await uploader.UploadAsync();

        await CompleteFilePartUploadAsync(fileId, version, eTags, fileType, cancellationToken);
        progressCallback?.Invoke(1);
    }

    private async ValueTask<string> PutFileAsync(string uploadUrl, Stream stream, HttpClient awsClient,
        string? md5 = null, bool isSimpleUpload = false, string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var content = new StreamContent(stream);
        if (isSimpleUpload)
        {
            if (md5 is null)
                throw new ArgumentNullException(nameof(md5), "MD5 should be provided for simple upload.");

            if (contentType is null)
                throw new ArgumentNullException(nameof(contentType), "Content type should be provided for simple upload.");

            content.Headers.ContentMD5 = Convert.FromBase64String(md5);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, uploadUrl)
        {
            Content = content
        };

        var response = await awsClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        if (response.Headers.ETag is null)
            throw new UnexpectedApiBehaviourException("Api did not return an ETag header.");

        return response.Headers.ETag.Tag;
    }
}