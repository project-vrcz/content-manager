using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.ViewModels.Pages;
using VRChatContentManager.Core.Management.Services;

namespace VRChatContentManager.App.ViewModels.ContentManager.Pages.Avatar;

public sealed partial class ContentManagerAvatarRootPageViewModel(
    AvatarContentManagementService avatarContentManagementService)
    : PageViewModelBase
{
    [RelayCommand]
    private async Task Load()
    {
        await avatarContentManagementService.GetAllAvatarsAsync();
    }
}