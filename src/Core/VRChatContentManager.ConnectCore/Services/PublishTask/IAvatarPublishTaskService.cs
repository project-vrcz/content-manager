namespace VRChatContentManager.ConnectCore.Services.PublishTask;

public interface IAvatarPublishTaskService
{
    Task CreatePublishTaskAsync(
        string avatarId,
        string avatarBundleFileId,
        string name,
        string platform,
        string unityVersion,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus
    );
}