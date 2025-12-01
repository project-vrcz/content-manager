namespace VRChatContentManager.Core.Models;

public enum ContentPublishTaskStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Cancelling,
    Canceled
}