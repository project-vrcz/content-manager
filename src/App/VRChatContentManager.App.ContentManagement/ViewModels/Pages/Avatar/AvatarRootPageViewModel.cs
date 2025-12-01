using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.ContentManagement.ViewModels.Data.List;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.Core.Management.Services;

namespace VRChatContentManager.App.ContentManagement.ViewModels.Pages.Avatar;

public sealed partial class AvatarRootPageViewModel(
    AvatarContentManagementService avatarContentManagementService,
    AvatarListItemViewModelFactory itemViewModelFactory)
    : PageViewModelBase
{
    [ObservableProperty] public partial AvaloniaList<AvatarListItemViewModel> Avatars { get; private set; } = [];

    [RelayCommand]
    private async Task Load()
    {
        var avatars = await avatarContentManagementService.GetAllAvatarsAsync();
        var viewModels = avatars.Select(itemViewModelFactory.Create).ToList();

        Avatars.Clear();
        Avatars.AddRange(viewModels);
    }

    [RelayCommand]
    private async Task RefreshAllAvatars()
    {
        await avatarContentManagementService.RefreshAllAvatarsAsync();
    }
}