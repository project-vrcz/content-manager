using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.ContentManagement.ViewModels.Pages.Avatar;
using VRChatContentManager.App.Shared;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels;
using VRChatContentManager.Core.Management.Models.Entity.Avatar;

namespace VRChatContentManager.App.ContentManagement.ViewModels.Data.Navigation.Avatar;

public sealed partial class AvatarQueryFilterNavigationItemViewModel(
    AvatarContentQueryFilterEntity avatarContentQueryFilterEntity,
    [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
    NavigationService navigationService,
    ContentManagerAvatarQueryFilterPageViewModelFactory pageViewModelFactory)
    : ViewModelBase, ITreeNavigationItemViewModel
{
    public string Name => avatarContentQueryFilterEntity.Name;
    public AvaloniaList<ITreeNavigationItemViewModel> Children { get; } = [];

    public bool Match(PageViewModelBase pageViewModel)
    {
        if (pageViewModel is not AvatarQueryFilterPageViewModel page)
            return false;

        return page.Id == avatarContentQueryFilterEntity.Id;
    }

    [RelayCommand]
    private void Navigate()
    {
        var page = pageViewModelFactory.Create(avatarContentQueryFilterEntity);
        navigationService.Navigate(page);
    }
}

public sealed class AvatarQueryFilterNavigationItemViewModelFactory(
    [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
    NavigationService navigationService,
    ContentManagerAvatarQueryFilterPageViewModelFactory pageViewModelFactory)
{
    public AvatarQueryFilterNavigationItemViewModel Create(
        AvatarContentQueryFilterEntity avatarContentQueryFilterEntity)
    {
        return new AvatarQueryFilterNavigationItemViewModel(avatarContentQueryFilterEntity, navigationService,
            pageViewModelFactory);
    }
}