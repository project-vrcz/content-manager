using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.Core.Models;
using VRChatContentManager.Core.Services.PublishTask;

namespace VRChatContentManager.App.ViewModels.Data.PublishTasks;

public sealed partial class PublishTaskViewModel(
    ContentPublishTaskService publishTaskService,
    TaskManagerService taskManagerService)
    : ViewModelBase
{
    public string TaskId => publishTaskService.TaskId;

    public string ContentId => publishTaskService.ContentId;
    public string ContentName => publishTaskService.ContentName;
    public string ContentType => publishTaskService.ContentType;
    public string ContentPlatform => publishTaskService.ContentPlatform;

    public string ProgressText => publishTaskService.ProgressText;
    public double? ProgressValue => publishTaskService.ProgressValue * 100;
    public bool IsIndeterminate => !ProgressValue.HasValue;

    public ContentPublishTaskStatus Status => publishTaskService.Status;

    [RelayCommand]
    private void Load()
    {
        publishTaskService.ProgressChanged += OnTaskProgressChanged;

        // to fix some kind of initial state not updated
        NotifyTaskChanged();
    }

    [RelayCommand]
    private void Unload()
    {
        publishTaskService.ProgressChanged -= OnTaskProgressChanged;
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await publishTaskService.CancelAsync();
    }

    [RelayCommand]
    private void Remove()
    {
        taskManagerService.RemoveTask(publishTaskService.TaskId);
    }

    [RelayCommand]
    private void Start()
    {
        publishTaskService.Start();
    }

    private void OnTaskProgressChanged(object? o, PublishTaskProgressEventArg publishTaskProgressEventArg)
    {
        Dispatcher.UIThread.Invoke(NotifyTaskChanged);
    }

    private void NotifyTaskChanged()
    {
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(ProgressValue));
        OnPropertyChanged(nameof(IsIndeterminate));
        OnPropertyChanged(nameof(Status));
    }
}

public sealed class PublishTaskViewModelFactory
{
    public PublishTaskViewModel Create(
        ContentPublishTaskService publishTaskService,
        TaskManagerService taskManagerService)
        => new(publishTaskService, taskManagerService);
}