using Microsoft.Extensions.Logging;
using VRChatContentManager.ConnectCore.Services;
using VRChatContentManager.Core.Models;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Models.VRChatApi.Rest.UnityPackages;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Worlds;
using VRChatContentManager.Core.Services.UserSession;
using VRChatContentManager.Core.Services.VRChatApi;
using VRChatContentManager.Core.Utils;

namespace VRChatContentManager.Core.Services.PublishTask.ContentPublisher;

public sealed class WorldContentPublisher : IContentPublisher
{
    private readonly string _worldId;
    private readonly string _worldName;
    private readonly string _platform;
    private readonly string _unityVersion;
    private readonly string? _worldSignature;
    private readonly int? _capacity;
    private readonly int? _recommendedCapacity;
    private readonly string? _previewYoutubeId;
    private readonly string[] _udonProducts;


    private readonly UserSessionService _userSessionService;

    private readonly ILogger<WorldContentPublisher> _logger;
    private readonly IFileService _tempFileService;

    private readonly VRChatApiClient _apiClient;

    public WorldContentPublisher(string worldId,
        string worldName,
        string platform,
        string unityVersion,
        string? worldSignature,
        int? capacity,
        int? recommendedCapacity,
        string? previewYoutubeId,
        string[]? udonProducts,
        UserSessionService userSessionService,
        ILogger<WorldContentPublisher> logger,
        IFileService tempFileService)
    {
        _worldId = worldId;
        _worldName = worldName;
        _platform = platform;
        _unityVersion = unityVersion;
        _worldSignature = worldSignature;
        _userSessionService = userSessionService;
        _logger = logger;
        _tempFileService = tempFileService;
        _capacity = capacity;
        _recommendedCapacity = recommendedCapacity;
        _previewYoutubeId = previewYoutubeId;
        _udonProducts = udonProducts ?? [];

        _apiClient = _userSessionService.GetApiClient();
    }

    public string GetContentType() => "world";

    public string GetContentName() => _worldName;
    public string GetContentPlatform() => _platform;

    private const long MaxBundleFileSizeForMobileBytes = 104857600; // 100 MB

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
            throw new InvalidOperationException("Could not find the provided bundle file.");

        if (thumbnailFile is null && thumbnailFileId is not null)
            throw new ArgumentException("Could not find the provided thumbnail file.", nameof(thumbnailFileId));

        if (!UnityBuildTargetUtils.IsStandalonePlatform(_platform) &&
            bundleFileStream.Length > MaxBundleFileSizeForMobileBytes)
            throw new ArgumentException(
                "The provided bundle file exceeds the maximum allowed size of 100 MB for this platform.",
                nameof(bundleFileId));

        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Publish World {WorldId}", _worldId);
        progressReporter?.Report("Fetching world detail...");

        // Step 1. Fetch world detail, if not found means we need to create a new world.
        VRChatApiWorld? world = null;
        try
        {
            world = await _apiClient.GetWorldAsync(_worldId);
        }
        catch (ApiErrorException ex) when (ex.StatusCode == 404)
        {
            _logger.LogInformation("The world {WorldId} was not found. Will create new world.", _worldId);
        }

        // Step 2. Try to get the asset file for this platform, if not create a new one.
        // This step also cleanups any incomplete file versions.
        var fileId = await GetOrCreateBundleFileIdAsync(world);

        _logger.LogInformation("Using file id {FileId} for world {WorldId}", fileId, _worldId);
        progressReporter?.Report("Preparing for upload bundle file...");

        // Step 3. Create and upload a new file version
        var fileVersion = await _apiClient.CreateAndUploadFileVersionAsync(
            bundleFileStream,
            fileId,
            VRChatApiFlieUtils.GetMimeTypeFromExtension(".vrcw"),
            awsClient,
            "World Bundle",
            arg => progressReporter?.Report(arg.ProgressText, arg.ProgressValue)
            , cancellationToken
        );

        // Step 3.1 Upload thumbnail if needed
        string? imageUri = null;
        if (thumbnailFile is not null && thumbnailFileStream is not null)
        {
            _logger.LogInformation("Uploading thumbnail for avatar {AvatarId}", _worldId);
            progressReporter?.Report("Uploading thumbnail...");

            var imageFileId = await GetOrCreateBundleImageIdAsync(world, thumbnailFile.FileName);

            var imageFileVersion = await _apiClient.CreateAndUploadFileVersionAsync(
                thumbnailFileStream,
                imageFileId,
                VRChatApiFlieUtils.GetMimeTypeFromExtension(Path.GetExtension(thumbnailFile.FileName)),
                awsClient,
                "World Thumbnail",
                arg => progressReporter?.Report(arg.ProgressText, arg.ProgressValue), cancellationToken
            );

            if (imageFileVersion.File is null)
                throw new UnexpectedApiBehaviourException(
                    "Api did not return file info for created image file version.");

            imageUri = imageFileVersion.File.Url;
        }

        if (fileVersion.File is null)
            throw new UnexpectedApiBehaviourException("Api did not return file info for created file version.");

        // Step 4. Update or Create World
        if (world is not null)
        {
            _logger.LogInformation("Updating world {WorldId} to use new file version {Version}", _worldId,
                fileVersion.Version);
            progressReporter?.Report("Updating world to latest asset version...");

            await _apiClient.CreateWorldVersionAsync(_worldId, new CreateWorldVersionRequest(
                _worldName,
                fileVersion.File.Url,
                fileVersion.Version,
                _platform,
                _unityVersion,
                _worldSignature,
                imageUri,
                description,
                tags,
                releaseStatus,
                _capacity,
                _recommendedCapacity,
                _previewYoutubeId,
                _udonProducts
            ), cancellationToken);
        }
        else
        {
            _logger.LogInformation("Creating new world {WorldId} with file version {Version}", _worldId,
                fileVersion.Version);
            progressReporter?.Report("Creating new world...");

            await _apiClient.CreateWorldAsync(new CreateWorldRequest(
                _worldId,
                _worldName,
                fileVersion.File.Url,
                4,
                _platform,
                _unityVersion,
                _worldSignature,
                imageUri,
                description,
                tags,
                releaseStatus,
                _capacity,
                _recommendedCapacity,
                _previewYoutubeId,
                _udonProducts
            ), cancellationToken);
        }

        _logger.LogInformation("Successfully published world {WorldId}", _worldId);
    }

    private VRChatApiUnityPackage? TryGetUnityPackageForPlatform(VRChatApiWorld world)
    {
        return world.UnityPackages
            .Where(package => package.Platform == _platform)
            .GroupBy(package => package.UnityVersion)
            .MaxBy(group => UnityVersion.TryParse(group.Key))!
            .MaxBy(package => package.AssetVersion);
    }

    private async ValueTask<string> GetOrCreateBundleFileIdAsync(VRChatApiWorld? apiWorld)
    {
        if (apiWorld is not null)
        {
            var platformApiUnityPackage = TryGetUnityPackageForPlatform(apiWorld);
            if (platformApiUnityPackage is not null)
            {
                var fileId = VRChatApiFlieUtils.TryGetFileIdFromAssetUrl(platformApiUnityPackage.AssetUrl);
                if (fileId is null)
                    throw new UnexpectedApiBehaviourException("Api returned an invalid asset url.");

                return fileId;
            }
        }

        var fileName = $"World - {_worldName} - Asset bundle - {_unityVersion}-{_platform}";
        var file = await _apiClient.CreateFileAsync(fileName, "application/x-world", ".vrcw");
        return file.Id;
    }

    private async ValueTask<string> GetOrCreateBundleImageIdAsync(VRChatApiWorld? apiWorld, string imageFileName)
    {
        if (apiWorld?.ImageUrl is not null)
        {
            var fileId = VRChatApiFlieUtils.TryGetFileIdFromAssetUrl(apiWorld.ImageUrl);
            if (fileId is null)
                throw new UnexpectedApiBehaviourException("Api returned an invalid image asset url.");

            return fileId;
        }

        var extension = Path.GetExtension(imageFileName);
        var mimeType = VRChatApiFlieUtils.GetMimeTypeFromExtension(extension);

        var fileName = $"World - {_worldName} - Image - {_unityVersion}-{_platform}";
        var file = await _apiClient.CreateFileAsync(fileName, mimeType, extension);
        return file.Id;
    }
}

public sealed class WorldContentPublisherFactory(ILogger<WorldContentPublisher> logger, IFileService tempFileService)
{
    public WorldContentPublisher Create(
        UserSessionService userSessionService,
        string worldId,
        string worldName,
        string platform,
        string unityVersion,
        string? worldSignature,
        int? capacity,
        int? recommendedCapacity,
        string? previewYoutubeId,
        string[]? udonProducts)
    {
        return new WorldContentPublisher(
            worldId,
            worldName,
            platform,
            unityVersion,
            worldSignature,
            capacity,
            recommendedCapacity,
            previewYoutubeId,
            udonProducts,
            userSessionService,
            logger,
            tempFileService
        );
    }
}