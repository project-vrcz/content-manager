using System;
using System.Linq;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.ViewModels.Data.Connect;
using VRChatContentManager.ConnectCore.Services.Connect.SessionStorage;

namespace VRChatContentManager.App.ViewModels.Settings;

public sealed partial class SessionsSettingsViewModel(
    ISessionStorageService sessionStorageService,
    RpcClientSessionViewModelFactory rpcClientSessionViewModelFactory) : ViewModelBase
{
    [ObservableProperty]
    public partial AvaloniaList<RpcClientSessionViewModel> ClientSessions { get; private set; } = [];

    [RelayCommand]
    private void Load()
    {
        UpdateSessions();
        sessionStorageService.SessionsChanged += OnSessionsChanged;
    }

    [RelayCommand]
    private void Unload()
    {
        sessionStorageService.SessionsChanged -= OnSessionsChanged;
    }

    private void UpdateSessions()
    {
        var sessions = sessionStorageService.GetAllSessions();
        var sessionViewModels = sessions.Select(rpcClientSessionViewModelFactory.Create).ToList();

        ClientSessions = new AvaloniaList<RpcClientSessionViewModel>(sessionViewModels);
    }

    private void OnSessionsChanged(object? sender, EventArgs e) => UpdateSessions();
}