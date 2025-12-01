using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.Core.Services.UserSession;

namespace VRChatContentManager.App.ViewModels.Pages.GettingStarted;

public partial class GuideSetupUnityPageViewModel(NavigationService navigationService) : PageViewModelBase
{
    [RelayCommand]
    private Task Load()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void Skip()
    {
        navigationService.Navigate<GuideFinishSetupPageViewModel>();
    }
}