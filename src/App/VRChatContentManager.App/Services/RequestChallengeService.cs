using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using VRChatContentManager.App.Messages.Connect;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.ViewModels.Dialogs;
using VRChatContentManager.ConnectCore.Services.Connect.Challenge;

namespace VRChatContentManager.App.Services;

public class RequestChallengeService(
    DialogService dialogService,
    RequestChallengeDialogViewModelFactory dialogViewModelFactory) : IRequestChallengeService
{
    public Task RequestChallengeAsync(string code, string clientId, string identityPrompt, string clientName)
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            await dialogService
                .ShowDialogAsync(
                    dialogViewModelFactory.Create(code, clientId, identityPrompt, clientName)).AsTask();
        });

        return Task.CompletedTask;
    }

    public Task CompleteChallengeAsync(string clientId)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            WeakReferenceMessenger.Default.Send(new ConnectChallengeCompletedMessage(clientId));
        });

        return Task.CompletedTask;
    }
}