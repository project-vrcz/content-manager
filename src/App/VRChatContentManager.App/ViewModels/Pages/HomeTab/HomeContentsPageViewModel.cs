using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.ContentManagement.Services;
using VRChatContentManager.App.ContentManagement.ViewModels;
using VRChatContentManager.App.ContentManagement.Views;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.Views;

namespace VRChatContentManager.App.ViewModels.Pages.HomeTab;

public sealed partial class HomeContentsPageViewModel(ContentManagerWindowService contentManagerWindowService)
    : PageViewModelBase
{
    [RelayCommand]
    private void OpenContentManagerWindow()
    {
        contentManagerWindowService.ShowContentManagerWindow();
    }
}