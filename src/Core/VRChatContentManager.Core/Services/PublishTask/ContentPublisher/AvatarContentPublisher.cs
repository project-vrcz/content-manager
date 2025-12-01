using Microsoft.Extensions.Logging;
using VRChatContentManager.ConnectCore.Services;
using VRChatContentManager.Core.Models;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Avatars;
using VRChatContentManager.Core.Models.VRChatApi.Rest.UnityPackages;
using VRChatContentManager.Core.Services.UserSession;
using VRChatContentManager.Core.Services.VRChatApi;
using VRChatContentManager.Core.Utils;

namespace VRChatContentManager.Core.Services.PublishTask.ContentPublisher;

public sealed class AvatarContentPublisher : IContentPublisher
{
    private readonly string _avatarId;
    private readonly string _name;
    private readonly string _platform;
    private readonly string _unityVersion;
    private readonly UserSessionService _userSessionService;
    private readonly ILogger<AvatarContentPublisher> _logger;
    private readonly IFileService _tempFileService;

    private readonly VRChatApiClient _apiClient;

    public AvatarContentPublisher(string avatarId,
        string name,
        string platform,
        string unityVersion,
        UserSessionService userSessionService,
        ILogger<AvatarContentPublisher> logger,
        IFileService tempFileService)
    {
        _avatarId = avatarId;
        _name = name;
        _platform = platform;
        _unityVersion = unityVersion;
        _userSessionService = userSessionService;
        _logger = logger;
        _tempFileService = tempFileService;

        _apiClient = _userSessionService.GetApiClient();
    }

    public string GetContentType() => "avatar";
    public string GetContentName() => _name;
    public string GetContentPlatform() => _platform;

    private const long MaxBundleFileSizeForDesktopBytes = 209715200; // 200 MB
    private const long MaxBundleFileSizeForMobileBytes = 10485760; // 10 MB

    public async ValueTask PublishAsync(
        string bundleFileId,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        HttpClient awsClient,
        PublishStageProgressReporter? progressReporter = null,
        CancellationToken cancellationToken = default)
    {
        await using var bundleFileStream = await _tempFileService.GetFileAsync(bundleFileId);
        var thumbnailFile = thumbnailFileId is not null
            ? await _tempFileService.GetFileWithNameAsync(thumbnailFileId)
            : null;
        await using var thumbnailFileStream = thumbnailFile?.FileStream;

        if (bundleFileStream is null)
            throw new ArgumentException("Could not find the provided bundle file.", nameof(bundleFileId));

        if (thumbnailFile is null && thumbnailFileId is not null)
            throw new ArgumentException("Could not find the provided thumbnail file.", nameof(thumbnailFileId));

        if (UnityBuildTargetUtils.IsStandalonePlatform(_platform))
        {
            if (bundleFileStream.Length > MaxBundleFileSizeForDesktopBytes)
                throw new ArgumentException(
                    "The provided bundle file exceeds the maximum allowed size of 200 MB for this platform.",
                    nameof(bundleFileId));
        }
        else
        {
            if (bundleFileStream.Length > MaxBundleFileSizeForMobileBytes)
                throw new ArgumentException(
                    "The provided bundle file exceeds the maximum allowed size of 10 MB for this platform.",
                    nameof(bundleFileId));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Step 1. Try to get the asset file for this platform, if not create a new one.
        // This step also cleanups any incomplete file versions.

        _logger.LogInformation("Publish Avatar {AvatarId}", _avatarId);
        progressReporter?.Report("Fetching avatar detail...");

        var avatar = await _apiClient.GetAvatarAsync(_avatarId, cancellationToken);
        var fileId = await GetOrCreateBundleFileIdAsync(avatar);

        // Step 2. Create and upload a new file version
        _logger.LogInformation("Using file id {FileId} for avatar {AvatarId}", fileId, _avatarId);
        progressReporter?.Report("Preparing for upload bundle file...");

        var fileVersion = await _apiClient.CreateAndUploadFileVersionAsync(
            bundleFileStream,
            fileId,
            VRChatApiFlieUtils.GetMimeTypeFromExtension(".vrca"),
            awsClient,
            "Avatar Bundle",
            arg => progressReporter?.Report(arg.ProgressText, arg.ProgressValue), cancellationToken
        );

        if (fileVersion.File is null)
            throw new UnexpectedApiBehaviourException("Api did not return file info for created file version.");

        // Step 2.1 Upload thumbnail if needed
        string? imageUri = null;
        if (thumbnailFile is not null && thumbnailFileStream is not null)
        {
            _logger.LogInformation("Uploading thumbnail for avatar {AvatarId}", _avatarId);
            progressReporter?.Report("Uploading thumbnail...");

            var imageFileId = await GetOrCreateBundleImageIdAsync(avatar, thumbnailFile.FileName);

            var imageFileVersion = await _apiClient.CreateAndUploadFileVersionAsync(
                thumbnailFileStream,
                imageFileId,
                VRChatApiFlieUtils.GetMimeTypeFromExtension(Path.GetExtension(thumbnailFile.FileName)),
                awsClient,
                "Avatar Thumbnail",
                arg => progressReporter?.Report(arg.ProgressText, arg.ProgressValue), cancellationToken
            );

            if (imageFileVersion.File is null)
                throw new UnexpectedApiBehaviourException(
                    "Api did not return file info for created image file version.");

            imageUri = imageFileVersion.File.Url;
        }

        // Step 3. Update Avatar
        _logger.LogInformation("Updating avatar {AvatarId} to use new file version {Version}", _avatarId,
            fileVersion.Version);
        progressReporter?.Report("Updating avatar to latest asset version...");

        await _apiClient.CreateAvatarVersionAsync(_avatarId, new CreateAvatarVersionRequest(
            _name,
            fileVersion.File.Url,
            1,
            _platform,
            _unityVersion,
            imageUri,
            description,
            tags,
            releaseStatus
        ), cancellationToken);

        _logger.LogInformation("Successfully published avatar {AvatarId}", _avatarId);
    }

    private VRChatApiUnityPackage? TryGetUnityPackageForPlatform(VRChatApiAvatar apiAvatar)
    {
        var platformApiUnityPackage = apiAvatar.UnityPackages
            .Where(package => package.Platform == _platform)
            .GroupBy(package => package.UnityVersion)
            .MaxBy(group => UnityVersion.TryParse(group.Key))?
            .MaxBy(package => package.AssetVersion);

        return platformApiUnityPackage;
    }

    private async ValueTask<string> GetOrCreateBundleFileIdAsync(VRChatApiAvatar apiAvatar)
    {
        var platformApiUnityPackage = TryGetUnityPackageForPlatform(apiAvatar);
        if (platformApiUnityPackage is not null)
        {
            var fileId = VRChatApiFlieUtils.TryGetFileIdFromAssetUrl(platformApiUnityPackage.AssetUrl);
            if (fileId is null)
                throw new UnexpectedApiBehaviourException("Api returned an invalid asset url.");

            return fileId;
        }

        var fileName = $"Avatar - {_name} - Asset bundle - {_unityVersion}-{_platform}";
        var file = await _apiClient.CreateFileAsync(fileName, "application/x-avatar", ".vrca");
        return file.Id;
    }

    private async ValueTask<string> GetOrCreateBundleImageIdAsync(VRChatApiAvatar apiAvatar, string imageFileName)
    {
        if (apiAvatar.ImageUrl is not null)
        {
            var fileId = VRChatApiFlieUtils.TryGetFileIdFromAssetUrl(apiAvatar.ImageUrl);
            if (fileId is null)
                throw new UnexpectedApiBehaviourException("Api returned an invalid image asset url.");

            return fileId;
        }

        var extension = Path.GetExtension(imageFileName);
        var mimeType = VRChatApiFlieUtils.GetMimeTypeFromExtension(extension);
        
        var fileName = $"Avatar - {_name} - Image - {_unityVersion}-{_platform}";
        var file = await _apiClient.CreateFileAsync(fileName, mimeType, extension);
        return file.Id;
    }
}

public sealed class AvatarContentPublisherFactory(ILogger<AvatarContentPublisher> logger, IFileService tempFileService)
{
    public AvatarContentPublisher Create(
        UserSessionService userSession,
        string avatarId,
        string name,
        string platform,
        string unityVersion)
    {
        return new AvatarContentPublisher(
            avatarId,
            name,
            platform,
            unityVersion,
            userSession,
            logger,
            tempFileService
        );
    }
}