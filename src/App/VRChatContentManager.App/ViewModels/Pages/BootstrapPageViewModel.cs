using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Pages.GettingStarted;
using VRChatContentManager.Core.Services.UserSession;
using VRChatContentManager.Core.Settings;
using VRChatContentManager.Core.Settings.Models;

namespace VRChatContentManager.App.ViewModels.Pages;

public sealed partial class BootstrapPageViewModel(
    UserSessionManagerService sessionManagerService,
    NavigationService navigationService,
    IWritableOptions<AppSettings> appSettings) : PageViewModelBase
{
    [RelayCommand]
    private async Task Load()
    {
        await sessionManagerService.RestoreSessionsAsync();
        
        if (!appSettings.Value.SkipFirstSetup)
        {
            navigationService.Navigate<GuideWelcomePageViewModel>();
            return;
        }

        navigationService.Navigate<HomePageViewModel>();
    }
}