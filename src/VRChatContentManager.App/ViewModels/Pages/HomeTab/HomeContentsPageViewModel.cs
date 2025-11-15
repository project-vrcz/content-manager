using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Views;

namespace VRChatContentManager.App.ViewModels.Pages.HomeTab;

public sealed partial class HomeContentsPageViewModel(ContentManagerWindow contentManagerWindow)
    : PageViewModelBase
{
    [RelayCommand]
    private void OpenContentManagerWindow()
    {
        contentManagerWindow.Show();
        contentManagerWindow.Activate();
    }
}