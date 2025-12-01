using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Shared.ViewModels.Dialogs;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Services.UserSession;

namespace VRChatContentManager.App.ViewModels.Dialogs;

public sealed partial class TwoFactorAuthDialogViewModel(UserSessionService userSessionService, bool isEmailOtp)
    : DialogViewModelBase
{
    [ObservableProperty] public partial string Code { get; set; } = "";
    public bool IsEmailOtp => isEmailOtp;

    [ObservableProperty] public partial bool HasError { get; private set; }
    [ObservableProperty] public partial string ErrorMessage { get; private set; } = "";

    [RelayCommand]
    private async Task Verify()
    {
        var apiClient = userSessionService.GetApiClient();

        bool isVerify;
        try
        {
            isVerify = await apiClient.VerifyOtpAsync(Code, IsEmailOtp);
        }
        catch (ApiErrorException ex)
        {
            HasError = true;
            ErrorMessage = ex.ApiErrorMessage;
            return;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
            return;
        }

        if (!isVerify)
        {
            HasError = true;
            ErrorMessage = "Invalid Code";
            return;
        }

        HasError = false;

        RequestClose(true);
    }
}

public sealed class TwoFactorAuthDialogViewModelFactory
{
    public TwoFactorAuthDialogViewModel Create(UserSessionService userSessionService, bool isEmailOtp)
    {
        return new TwoFactorAuthDialogViewModel(userSessionService, isEmailOtp);
    }
}