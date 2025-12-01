namespace VRChatContentManager.Core.Services.PublishTask.BundleProcesser;

public interface IBundleProcesser
{
    ValueTask<string> ProcessBundleAsync(
        string bundleFileId,
        PublishStageProgressReporter? progressReporter = null,
        CancellationToken cancellationToken = default
    );
}