using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.ConnectCore.Models.ClientSession;
using VRChatContentManager.ConnectCore.Services.Connect.SessionStorage;

namespace VRChatContentManager.App.ViewModels.Data.Connect;

public sealed partial class RpcClientSessionViewModel(
    RpcClientSession clientSession,
    ISessionStorageService sessionStorageService) : ViewModelBase
{
    public string ClientId => clientSession.ClientId;
    public string ClientName => clientSession.ClientName;
    public DateTimeOffset Expires => clientSession.Expires;
    public DateTime ExpiresLocal => clientSession.Expires.LocalDateTime;

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await sessionStorageService.RemoveSessionByClientIdAsync(clientSession.ClientId);
    }
}

public sealed class RpcClientSessionViewModelFactory(
    ISessionStorageService sessionStorageService)
{
    public RpcClientSessionViewModel Create(RpcClientSession clientSession)
    {
        return new RpcClientSessionViewModel(clientSession, sessionStorageService);
    }
}