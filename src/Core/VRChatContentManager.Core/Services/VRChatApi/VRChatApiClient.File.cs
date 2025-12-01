using System.Net.Http.Json;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Models.VRChatApi.Rest;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Files;

namespace VRChatContentManager.Core.Services.VRChatApi;

public sealed partial class VRChatApiClient
{
    public async ValueTask<VRChatApiFile> GetFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var response = await httpClient.GetAsync($"file/{fileId}", cancellationToken);
        await HandleErrorResponseAsync(response);

        var file = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.VRChatApiFile,
            cancellationToken: cancellationToken);
        if (file is null)
            throw new UnexpectedApiBehaviourException("The API returned a null file.");

        return file;
    }

    public async ValueTask<VRChatApiFile> CreateFileAsync(
        string fileName,
        string mimeType,
        string extension,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        var request = new HttpRequestMessage(HttpMethod.Post, "file")
        {
            Content = JsonContent.Create(
                new CreateFileRequest(fileName, mimeType, extension),
                ApiJsonContext.Default.CreateFileRequest)
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        await HandleErrorResponseAsync(response);

        var file = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.VRChatApiFile,
            cancellationToken: cancellationToken);
        if (file is null)
            throw new UnexpectedApiBehaviourException("The API returned a null file.");

        return file;
    }

    public async ValueTask<VRChatApiFileVersion> CreateFileVersionAsync(
        string fileId,
        string fileMd5,
        long fileSizeInBytes,
        string signatureMd5,
        long signatureSizeInBytes,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new HttpRequestMessage(HttpMethod.Post, "file/" + fileId)
        {
            Content = JsonContent.Create(
                new CreateFileVersionRequest(fileMd5, fileSizeInBytes, signatureMd5, signatureSizeInBytes),
                ApiJsonContext.Default.CreateFileVersionRequest)
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        await HandleErrorResponseAsync(response);

        var file = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.VRChatApiFile,
            cancellationToken: cancellationToken);
        if (file is null)
            throw new UnexpectedApiBehaviourException("The API returned a null file.");

        var latestVersion = file.Versions.MaxBy(v => v.Version);
        if (latestVersion is null)
            throw new UnexpectedApiBehaviourException("The API returned a file without versions.");

        if (latestVersion.Version == 0)
            throw new UnexpectedApiBehaviourException(
                "The API returned a file with no version created (only version 0).");

        return latestVersion;
    }

    public async ValueTask DeleteFileVersionAsync(string fileId, long versionId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var response = await httpClient.DeleteAsync($"file/{fileId}/{versionId}", cancellationToken);
        await HandleErrorResponseAsync(response);
    }

    public async ValueTask<FileVersionUploadStatus> GetFileVersionUploadStatusAsync(string fileId, int version,
        VRChatApiFileType fileType = VRChatApiFileType.File)
    {
        var response = await httpClient.GetAsync($"file/{fileId}/version/{version}/{fileType.ToApiString()}/status");

        await HandleErrorResponseAsync(response);

        var status = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.FileVersionUploadStatus);
        if (status is null)
            throw new UnexpectedApiBehaviourException("The API returned a null file version upload status.");

        return status;
    }

    public async ValueTask<string> GetSimpleUploadUrlAsync(string fileId, int version,
        VRChatApiFileType fileType = VRChatApiFileType.File, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new HttpRequestMessage(HttpMethod.Put, $"file/{fileId}/{version}/{fileType.ToApiString()}/start");
        var response = await httpClient.SendAsync(request, cancellationToken);

        await HandleErrorResponseAsync(response);

        var uploadUrl =
            await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.FileUploadUrlResponse, cancellationToken);

        if (uploadUrl is null)
            throw new UnexpectedApiBehaviourException("The API returned a null file upload url object.");

        return uploadUrl.Url;
    }

    public async ValueTask<string> GetFilePartUploadUrlAsync(string fileId, int version, int partNumber = 1,
        VRChatApiFileType fileType = VRChatApiFileType.File, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new HttpRequestMessage(HttpMethod.Put,
            $"file/{fileId}/{version}/{fileType.ToApiString()}/start?partNumber={partNumber}");
        var response = await httpClient.SendAsync(request, cancellationToken);

        await HandleErrorResponseAsync(response);

        var uploadUrl = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.FileUploadUrlResponse,
            cancellationToken: cancellationToken);

        if (uploadUrl is null)
            throw new UnexpectedApiBehaviourException("The API returned a null file upload url object.");

        return uploadUrl.Url;
    }

    public async ValueTask CompleteSimpleFileUploadAsync(string fileId, int version,
        VRChatApiFileType fileType = VRChatApiFileType.File, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request =
            new HttpRequestMessage(HttpMethod.Put, $"file/{fileId}/{version}/{fileType.ToApiString()}/finish");

        var response = await httpClient.SendAsync(request, cancellationToken);

        await HandleErrorResponseAsync(response);
    }

    public async ValueTask CompleteFilePartUploadAsync(string fileId, int version,
        string[]? eTags = null, VRChatApiFileType fileType = VRChatApiFileType.File,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request =
            new HttpRequestMessage(HttpMethod.Put, $"file/{fileId}/{version}/{fileType.ToApiString()}/finish");

        if (eTags is not null)
        {
            if (fileType == VRChatApiFileType.Signature)
                throw new ArgumentException("ETags are not required for signature file type.", nameof(eTags));

            request.Content = JsonContent.Create(new CompleteFileUploadRequest(eTags),
                ApiJsonContext.Default.CompleteFileUploadRequest);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);

        await HandleErrorResponseAsync(response);
    }

    public static async ValueTask<bool> CleanupIncompleteFileVersionsAsync(VRChatApiFile file,
        VRChatApiClient apiClient,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var incompleteVersions = file.Versions.Where(version => version.Status != "complete")
            .ToArray();
        foreach (var version in incompleteVersions)
        {
            await apiClient.DeleteFileVersionAsync(file.Id, version.Version, cancellationToken);
        }

        return incompleteVersions.Length == 0;
    }
}