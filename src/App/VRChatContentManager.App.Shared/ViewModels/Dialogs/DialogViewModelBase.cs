using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.ViewModels;

namespace VRChatContentManager.App.Shared.ViewModels.Dialogs;

public abstract partial class DialogViewModelBase : ViewModelBase
{
    public event EventHandler<object?>? CloseRequested;

    [RelayCommand]
    protected void RequestClose(object? arg = null)
    {
        CloseRequested?.Invoke(this, arg);
    }
}