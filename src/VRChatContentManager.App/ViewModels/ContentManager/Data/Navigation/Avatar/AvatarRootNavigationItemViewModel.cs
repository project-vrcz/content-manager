using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.ViewModels.ContentManager.Pages.Avatar;
using VRChatContentManager.App.ViewModels.Pages;

namespace VRChatContentManager.App.ViewModels.ContentManager.Data.Navigation.Avatar;

public sealed partial class AvatarRootNavigationItemViewModel(
    [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
    NavigationService navigationService)
    : ViewModelBase, ITreeNavigationItemViewModel
{
    public string Name => "Avatars";
    public AvaloniaList<ITreeNavigationItemViewModel> Children { get; } = [];

    public bool Match(PageViewModelBase pageViewModel) => pageViewModel is ContentManagerAvatarRootPageViewModel;

    [RelayCommand]
    private void Navigate() => navigationService.Navigate<ContentManagerAvatarRootPageViewModel>();
}