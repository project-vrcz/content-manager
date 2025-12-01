using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.Shared;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels;

namespace VRChatContentManager.App.ContentManagement.ViewModels.Data.Navigation;

public sealed partial class TreeNavigationItemViewModel<T>(
    string name,
    AvaloniaList<ITreeNavigationItemViewModel> children,
    NavigationService navigationService) : ViewModelBase, ITreeNavigationItemViewModel
    where T : PageViewModelBase
{
    public string Name => name;
    public AvaloniaList<ITreeNavigationItemViewModel> Children => children;

    public bool Match(PageViewModelBase pageViewModel)
    {
        return typeof(T) == pageViewModel.GetType();
    }

    [RelayCommand]
    private void Navigate()
    {
        navigationService.Navigate<T>();
    }
}

public sealed class TreeNavigationItemViewModelFactory(
    [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
    NavigationService navigationService)
{
    public TreeNavigationItemViewModel<T> Create<T>(
        string name,
        AvaloniaList<ITreeNavigationItemViewModel>? children = null)
        where T : PageViewModelBase
    {
        return new TreeNavigationItemViewModel<T>(name, children ?? [], navigationService);
    }
}