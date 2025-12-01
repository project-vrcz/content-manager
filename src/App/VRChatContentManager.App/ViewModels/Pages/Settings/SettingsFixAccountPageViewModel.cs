using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Dialogs;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Services.UserSession;

namespace VRChatContentManager.App.ViewModels.Pages.Settings;

public sealed partial class SettingsFixAccountPageViewModel(
    UserSessionService userSessionService,
    NavigationService navigationService,
    TwoFactorAuthDialogViewModelFactory twoFactorAuthDialogViewModelFactory,
    DialogService dialogService) : PageViewModelBase
{
    public string Username => userSessionService.UserNameOrEmail;
    [ObservableProperty] public partial string Password { get; set; } = "";

    [RelayCommand]
    private void BackToSettings()
    {
        navigationService.Navigate<SettingsPageViewModel>();
    }

    [RelayCommand]
    private async Task Login()
    {
        var loginResult = await userSessionService.LoginAsync(Password);

        if (loginResult.Requires2Fa.Length != 0 && (loginResult.Requires2Fa.Contains(Requires2FA.Totp) ||
                                                    loginResult.Requires2Fa.Contains(Requires2FA.EmailOtp)))
        {
            var isEmailOtp = loginResult.Requires2Fa.Contains(Requires2FA.EmailOtp);
            var result = await OpenTwoFactorAuthDialog(isEmailOtp);

            if (!result)
            {
                try
                {
                    await userSessionService.LogoutAsync();
                }
                catch
                {
                    // ignored
                }

                return;
            }
        }

        await userSessionService.GetCurrentUserAsync();
        navigationService.Navigate<SettingsPageViewModel>();
    }

    private async ValueTask<bool> OpenTwoFactorAuthDialog(bool isEmailOtp)
    {
        var dialog = twoFactorAuthDialogViewModelFactory.Create(userSessionService, isEmailOtp);

        var result = await dialogService.ShowDialogAsync(dialog);
        return result is true;
    }
}

public sealed class SettingsFixAccountPageViewModelFactory(
    NavigationService navigationService,
    TwoFactorAuthDialogViewModelFactory twoFactorAuthDialogViewModelFactory,
    DialogService dialogService)
{
    public SettingsFixAccountPageViewModel Create(UserSessionService userSessionService)
    {
        return new SettingsFixAccountPageViewModel(userSessionService,
            navigationService,
            twoFactorAuthDialogViewModelFactory,
            dialogService);
    }
}