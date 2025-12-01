namespace VRChatContentManager.Core.Models;

public sealed class PublishTaskProgressEventArg(string progressText, double? progressValue, ContentPublishTaskStatus status) : EventArgs
{
    public string ProgressText => progressText;
    public double? ProgressValue => progressValue;
    public ContentPublishTaskStatus Status => status;
}