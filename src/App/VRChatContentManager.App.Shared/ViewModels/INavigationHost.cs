using VRChatContentManager.App.Shared.ViewModels.Pages;

namespace VRChatContentManager.App.Shared.ViewModels;

public interface INavigationHost
{
    public void Navigate(PageViewModelBase pageViewModel);
}