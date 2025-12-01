using VRChatContentManager.Core.Models;
using VRChatContentManager.Core.Services.PublishTask.ContentPublisher;

namespace VRChatContentManager.Core.Services.PublishTask;

public sealed class TaskManagerService(ContentPublishTaskFactory contentPublishTaskFactory)
{
    public IReadOnlyDictionary<string, ContentPublishTaskService> Tasks => _tasks.AsReadOnly();
    private readonly Dictionary<string, ContentPublishTaskService> _tasks = [];

    public event EventHandler<ContentPublishTaskService>? TaskCreated;
    public event EventHandler<ContentPublishTaskService>? TaskRemoved;

    public async ValueTask<ContentPublishTaskService> CreateTask(
        string contentId,
        string bundleFileId,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        IContentPublisher contentPublisher
    )
    {
        var taskId = Guid.NewGuid().ToString("D");
        var task = await contentPublishTaskFactory.Create(taskId,
            contentId, bundleFileId, thumbnailFileId, description, tags, releaseStatus,
            contentPublisher);

        _tasks.Add(taskId, task);
        TaskCreated?.Invoke(this, task);
        return task;
    }

    public bool RemoveTask(string taskId)
    {
        if (!_tasks.TryGetValue(taskId, out var task))
            return false;

        if (task.Status != ContentPublishTaskStatus.Failed &&
            task.Status != ContentPublishTaskStatus.Completed &&
            task.Status != ContentPublishTaskStatus.Canceled)
            return false;

        _tasks.Remove(taskId);
        TaskRemoved?.Invoke(this, task);
        return true;
    }
}