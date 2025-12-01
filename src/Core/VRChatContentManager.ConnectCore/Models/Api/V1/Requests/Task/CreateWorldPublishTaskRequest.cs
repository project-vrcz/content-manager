namespace VRChatContentManager.ConnectCore.Models.Api.V1.Requests.Task;

public sealed class CreateWorldPublishTaskRequest
{
    public required string WorldId { get; set; }
    public required string Name { get; set; }
    public required string WorldBundleFileId { get; set; }
    public required string Platform { get; set; }
    public required string UnityVersion { get; set; }
    public string? AuthorId { get; set; }
    public string? WorldSignature { get; set; }
    public string? ThumbnailFileId { get; set; }
    public string? Description { get; set; }
    public string[] Tags { get; set; } = [];
    public string? ReleaseStatus { get; set; }
    public int? Capacity { get; set; }
    public int? RecommendedCapacity { get; set; }
    public string? PreviewYoutubeId { get; set; }
    public string[]? UdonProducts { get; set; }
}