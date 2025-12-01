using System.Linq;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.Core.Models;
using VRChatContentManager.Core.Services.PublishTask;

namespace VRChatContentManager.App.ViewModels.Data.PublishTasks;

public sealed partial class PublishTaskManagerViewModel(
    TaskManagerService taskManagerService,
    PublishTaskViewModelFactory taskFactory,
    string userDisplayName)
    : ViewModelBase, IPublishTaskManagerViewModel
{
    public string UserDisplayName { get; } = userDisplayName;
    public AvaloniaList<PublishTaskViewModel> Tasks { get; } = [];

    [RelayCommand]
    private void Load()
    {
        var viewModels = taskManagerService.Tasks.Select(task =>
                taskFactory.Create(task.Value, taskManagerService))
            .ToArray();

        Tasks.Clear();
        Tasks.AddRange(viewModels);

        taskManagerService.TaskCreated += OnTaskCreated;
        taskManagerService.TaskRemoved += OnTaskRemoved;
    }

    [RelayCommand]
    private void Unload()
    {
        taskManagerService.TaskCreated -= OnTaskCreated;
        taskManagerService.TaskRemoved -= OnTaskRemoved;
    }

    [RelayCommand]
    private void RemoveCompletedTasks()
    {
        var completedTasks = Tasks
            .Where(t => t.Status is ContentPublishTaskStatus.Completed)
            .ToArray();

        foreach (var task in completedTasks)
        {
            taskManagerService.RemoveTask(task.TaskId);
        }
    }

    [RelayCommand]
    private void RemoveFailedTasks()
    {
        var completedTasks = Tasks
            .Where(t => t.Status is ContentPublishTaskStatus.Failed)
            .ToArray();

        foreach (var task in completedTasks)
        {
            taskManagerService.RemoveTask(task.TaskId);
        }
    }

    [RelayCommand]
    private void RemoveCancelledTasks()
    {
        var completedTasks = Tasks
            .Where(t => t.Status is ContentPublishTaskStatus.Canceled)
            .ToArray();

        foreach (var task in completedTasks)
        {
            taskManagerService.RemoveTask(task.TaskId);
        }
    }

    [RelayCommand]
    private void RemoveAllRemovableTasks()
    {
        var completedTasks = Tasks
            .Where(t =>
                t.Status is ContentPublishTaskStatus.Completed or
                    ContentPublishTaskStatus.Failed or
                    ContentPublishTaskStatus.Canceled)
            .ToArray();

        foreach (var task in completedTasks)
        {
            taskManagerService.RemoveTask(task.TaskId);
        }
    }

    private void OnTaskCreated(object? _, ContentPublishTaskService task)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var viewModel = taskFactory.Create(task, taskManagerService);
            Tasks.Add(viewModel);
        });
    }

    private void OnTaskRemoved(object? sender, ContentPublishTaskService e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var viewModel = Tasks.FirstOrDefault(t => t.TaskId == e.TaskId);
            if (viewModel != null)
                Tasks.Remove(viewModel);
        });
    }
}

public sealed class PublishTaskManagerViewModelFactory(PublishTaskViewModelFactory taskFactory)
{
    public PublishTaskManagerViewModel Create(TaskManagerService taskManagerService, string userDisplayName) =>
        new(taskManagerService, taskFactory, userDisplayName);
}