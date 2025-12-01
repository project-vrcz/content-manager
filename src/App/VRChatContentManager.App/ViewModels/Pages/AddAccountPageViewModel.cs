using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Dialogs;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Services.UserSession;

namespace VRChatContentManager.App.ViewModels.Pages;

public sealed partial class AddAccountPageViewModel(
    TwoFactorAuthDialogViewModelFactory twoFactorAuthDialogFactory,
    UserSessionManagerService userSessionManagerService,
    DialogService dialogService,
    Action onRequestBack,
    Action onRequestDone)
    : PageViewModelBase
{
    [ObservableProperty] public partial string Username { get; set; } = "";
    [ObservableProperty] public partial string Password { get; set; } = "";

    [ObservableProperty] public partial bool HasError { get; private set; }
    [ObservableProperty] public partial string ErrorMessage { get; private set; } = "";

    [RelayCommand]
    private void Back()
    {
        onRequestBack();
    }

    [RelayCommand]
    private async Task Login()
    {
        if (userSessionManagerService.IsSessionExists(Username))
        {
            SetError("An account with this username/email already exists.");
            return;
        }

        var session = userSessionManagerService.CreateOrGetSession(Username);

        LoginResult loginResult;
        try
        {
            loginResult = await session.LoginAsync(Password);
        }
        catch (ApiErrorException ex)
        {
            await userSessionManagerService.RemoveSessionAsync(session);
            SetError(ex.ApiErrorMessage);
            return;
        }
        catch (Exception ex)
        {
            await userSessionManagerService.RemoveSessionAsync(session);
            SetError(ex.Message);
            return;
        }

        ClearError();

        if (loginResult.Requires2Fa.Length != 0 && (loginResult.Requires2Fa.Contains(Requires2FA.Totp) ||
                                                    loginResult.Requires2Fa.Contains(Requires2FA.EmailOtp)))
        {
            var isEmailOtp = loginResult.Requires2Fa.Contains(Requires2FA.EmailOtp);
            var result = await OpenTwoFactorAuthDialog(session, isEmailOtp);

            if (!result)
            {
                try
                {
                    await userSessionManagerService.RemoveSessionAsync(session);
                }
                catch
                {
                    // ignored
                }

                return;
            }
        }

        try
        {
            await userSessionManagerService.HandleSessionAfterLogin(session);
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
            return;
        }

        onRequestDone();
    }

    private async ValueTask<bool> OpenTwoFactorAuthDialog(UserSessionService userSessionService, bool isEmailOtp)
    {
        var dialog = twoFactorAuthDialogFactory.Create(userSessionService, isEmailOtp);

        var result = await dialogService.ShowDialogAsync(dialog);
        return result is true;
    }

    private void ClearError()
    {
        ErrorMessage = "";
        HasError = false;
    }

    private void SetError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private async ValueTask TryLogout(UserSessionService sessionService)
    {
        try
        {
            await sessionService.LogoutAsync();
        }
        catch
        {
        }
    }
}

public sealed class AddAccountPageViewModelFactory(
    TwoFactorAuthDialogViewModelFactory twoFactorAuthDialogFactory,
    UserSessionManagerService userSessionManagerService,
    DialogService dialogService)
{
    public AddAccountPageViewModel Create(
        Action onRequestBack,
        Action onRequestDone)
    {
        return new AddAccountPageViewModel(
            twoFactorAuthDialogFactory,
            userSessionManagerService,
            dialogService,
            onRequestBack,
            onRequestDone);
    }
}