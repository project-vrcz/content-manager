using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Data.PublishTasks;
using VRChatContentManager.Core.Services.PublishTask;
using VRChatContentManager.Core.Services.UserSession;

namespace VRChatContentManager.App.ViewModels.Pages.HomeTab;

public sealed partial class HomeTasksPageViewModel(
    PublishTaskManagerViewModelFactory managerViewModelFactory,
    UserSessionManagerService userSessionManagerService,
    ILogger<HomeTasksPageViewModel> logger) : PageViewModelBase
{
    public ObservableCollection<IPublishTaskManagerViewModel> TaskManagers { get; } = [];

    [RelayCommand]
    private async Task Load()
    {
        foreach (var session in userSessionManagerService.Sessions)
        {
            try
            {
                var scope = await session.CreateOrGetSessionScopeAsync();
                var managerService = scope.ServiceProvider.GetRequiredService<TaskManagerService>();

                var managerViewModel = managerViewModelFactory.Create(
                    managerService,
                    session.CurrentUser?.DisplayName ?? session.UserNameOrEmail
                );

                TaskManagers.Add(managerViewModel);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to get task manager for user session {UserNameOrEmail}", session.UserNameOrEmail);
                TaskManagers.Add(new InvalidSessionTaskManagerViewModel(ex, session));
            }
        }
    }
}