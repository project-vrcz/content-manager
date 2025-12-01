using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels.Pages;

namespace VRChatContentManager.App.ViewModels.Pages.GettingStarted;

public sealed partial class GuideFinishSetupPageViewModel(NavigationService navigationService) : PageViewModelBase
{
    [RelayCommand]
    private void Finish()
    {
        navigationService.Navigate<HomePageViewModel>();
    }
}