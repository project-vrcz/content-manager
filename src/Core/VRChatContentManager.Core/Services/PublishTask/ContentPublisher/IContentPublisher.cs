namespace VRChatContentManager.Core.Services.PublishTask.ContentPublisher;

public interface IContentPublisher
{
    string GetContentType();
    string GetContentName();
    string GetContentPlatform();

    ValueTask PublishAsync(
        string bundleFileId,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        HttpClient awsClient,
        PublishStageProgressReporter progressReporter,
        CancellationToken cancellationToken = default
    );
}