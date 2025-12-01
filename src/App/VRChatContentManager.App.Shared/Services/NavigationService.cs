using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.Shared.ViewModels;
using VRChatContentManager.App.Shared.ViewModels.Pages;

namespace VRChatContentManager.App.Shared.Services;

public class NavigationService(IServiceProvider serviceProvider)
{
    private INavigationHost? _navigationHost;

    public void Register(INavigationHost navigationHost)
    {
        _navigationHost = navigationHost;
    }

    public void Navigate(PageViewModelBase pageViewModel)
    {
        _navigationHost?.Navigate(pageViewModel);
    }

    public void Navigate<T>() where T : PageViewModelBase
    {
        var pageViewModel = serviceProvider.GetRequiredService<T>();
        Navigate(pageViewModel);
    }
}