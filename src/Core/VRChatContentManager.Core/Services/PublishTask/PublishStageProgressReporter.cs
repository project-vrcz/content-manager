namespace VRChatContentManager.Core.Services.PublishTask;

public sealed class PublishStageProgressReporter(Action<string, double?> progressReporter)
{
    public void Report(string progressText, double? progressValue = null)
    {
        progressReporter(progressText, progressValue);
    }
}