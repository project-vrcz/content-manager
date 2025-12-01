using System.Threading.Tasks;
using CliWrap;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VRChatContentManager.App.Messages.Connect;
using VRChatContentManager.App.Shared.ViewModels.Dialogs;

namespace VRChatContentManager.App.ViewModels.Dialogs;

public sealed partial class RequestChallengeDialogViewModel(
    string code,
    string clientId,
    string identityPrompt,
    string clientName) : DialogViewModelBase
{
    public string Code { get; } = code;
    public string ClientId { get; } = clientId;
    public string IdentityPrompt { get; } = identityPrompt;
    public string ClientName { get; } = clientName;

    [RelayCommand]
    private void Load()
    {
        WeakReferenceMessenger.Default.Register<ConnectChallengeCompletedMessage>(this,
            (_, message) =>
            {
                if (message.ClientId != ClientId)
                    return;

                WeakReferenceMessenger.Default.Unregister<ConnectChallengeCompletedMessage>(this);
                RequestClose();
            });
    }

    [RelayCommand]
    private void Unload()
    {
        WeakReferenceMessenger.Default.Unregister<ConnectChallengeCompletedMessage>(this);
    }
}

public sealed class RequestChallengeDialogViewModelFactory
{
    public RequestChallengeDialogViewModel Create(string code, string clientId, string identityPrompt, string clientName)
    {
        return new RequestChallengeDialogViewModel(code, clientId, identityPrompt, clientName);
    }
}