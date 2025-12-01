using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.ContentManagement.ViewModels.Pages.Avatar;
using VRChatContentManager.App.Shared;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels;
using VRChatContentManager.Core.Management.Services;

namespace VRChatContentManager.App.ContentManagement.ViewModels.Data.Navigation.Avatar;

public sealed partial class AvatarRootNavigationItemViewModel(
    [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
    NavigationService navigationService,
    AvatarContentManagementService avatarContentManagementService,
    AvatarQueryFilterNavigationItemViewModelFactory queryFilterNavigationItemViewModelFactory)
    : ViewModelBase, ITreeNavigationItemViewModel
{
    public string Name => "Avatars";
    public AvaloniaList<ITreeNavigationItemViewModel> Children { get; } = [];

    public bool Match(PageViewModelBase pageViewModel) => pageViewModel is AvatarRootPageViewModel;

    [RelayCommand]
    private void Navigate() => navigationService.Navigate<AvatarRootPageViewModel>();

    public async Task LoadChildrenAsync()
    {
        Children.Clear();

        var filters = await avatarContentManagementService.GetAllQueryFiltersAsync();
        var viewModels = filters.Select(queryFilterNavigationItemViewModelFactory.Create).ToArray();

        Children.AddRange(viewModels);
    }
}