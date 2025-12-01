using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Pages;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.ViewModels.Data;
using VRChatContentManager.App.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Pages.Settings;
using VRChatContentManager.Core.Services.UserSession;

namespace VRChatContentManager.App.ViewModels.Settings;

public sealed partial class AccountsSettingsViewModel(
    UserSessionManagerService userSessionManagerService,
    UserSessionViewModelFactory userSessionViewModelFactory,
    NavigationService navigationService,
    AddAccountPageViewModelFactory addAccountPageViewModelFactory
) : ViewModelBase
{
    [ObservableProperty] public partial AvaloniaList<UserSessionViewModel> UserSessions { get; private set; } = [];

    [RelayCommand]
    private void Load()
    {
        UpdateSessions();

        userSessionManagerService.SessionCreated += OnSessionCreated;
        userSessionManagerService.SessionRemoved += OnSessionRemoved;
    }

    [RelayCommand]
    private void Unload()
    {
        userSessionManagerService.SessionCreated -= OnSessionCreated;
        userSessionManagerService.SessionRemoved -= OnSessionRemoved;
    }

    private void OnSessionRemoved(object? sender, UserSessionService e) => UpdateSessions();
    private void OnSessionCreated(object? sender, UserSessionService e) => UpdateSessions();

    private void UpdateSessions()
    {
        UserSessions.Clear();
        var viewModels = userSessionManagerService.Sessions
            .Select(userSessionViewModelFactory.Create)
            .ToArray();

        UserSessions.AddRange(viewModels);
    }

    [RelayCommand]
    private void AddNewAccount()
    {
        var addAccountPageViewModel = addAccountPageViewModelFactory.Create(
            navigationService.Navigate<SettingsPageViewModel>,
            navigationService.Navigate<SettingsPageViewModel>);

        navigationService.Navigate(addAccountPageViewModel);
    }
}