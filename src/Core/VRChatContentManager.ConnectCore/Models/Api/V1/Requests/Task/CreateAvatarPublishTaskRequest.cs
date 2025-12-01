namespace VRChatContentManager.ConnectCore.Models.Api.V1.Requests.Task;

public sealed class CreateAvatarPublishTaskRequest
{
    public required string AvatarId { get; set; }
    public required string Name { get; set; }
    public required string AvatarBundleFileId { get; set; }
    public required string Platform { get; set; }
    public required string UnityVersion { get; set; }

    public string? ThumbnailFileId { get; set; }
    public string? Description { get; set; }
    public string[] Tags { get; set; } = [];
    public string? ReleaseStatus { get; set; }
}