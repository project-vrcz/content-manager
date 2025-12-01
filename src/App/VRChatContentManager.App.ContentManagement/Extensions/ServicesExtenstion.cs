using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.ContentManagement.Services;
using VRChatContentManager.App.ContentManagement.ViewModels;
using VRChatContentManager.App.ContentManagement.ViewModels.Data.List;
using VRChatContentManager.App.ContentManagement.ViewModels.Data.Navigation;
using VRChatContentManager.App.ContentManagement.ViewModels.Data.Navigation.Avatar;
using VRChatContentManager.App.ContentManagement.ViewModels.Dialogs.Data.Avatar;
using VRChatContentManager.App.ContentManagement.ViewModels.Pages;
using VRChatContentManager.App.ContentManagement.ViewModels.Pages.Avatar;
using VRChatContentManager.App.ContentManagement.Views;
using VRChatContentManager.App.Shared;
using VRChatContentManager.App.Shared.Services;

namespace VRChatContentManager.App.ContentManagement.Extensions;

public static class ServicesExtenstion
{
    public static IServiceCollection AddAppContentManagement(this IServiceCollection services)
    {
        services.AddSingleton<ContentManagerWindowService>();

        // ViewModels
        services.AddKeyedSingleton<NavigationService>(ServicesKeys.ContentManagerWindows);

        services.AddSingleton<ContentManagerWindowViewModel>();
        services.AddSingleton<ContentManagerWindow>();

        services.AddSingleton<MainViewModel>();

        // Pages
        services.AddTransient<HomePageViewModel>();
        services.AddTransient<AvatarRootPageViewModel>();

        services.AddTransient<ContentManagerAvatarQueryFilterPageViewModelFactory>();
        
        // Dialogs
        services.AddKeyedSingleton<DialogService>(ServicesKeys.ContentManagerWindows);
        
        services.AddTransient<EditAvatarLocalTagsDialogViewModelFactory>();

        // Navigation
        services.AddTransient<TreeNavigationItemViewModelFactory>();
        services.AddTransient<AvatarRootNavigationItemViewModel>();
        services.AddTransient<AvatarQueryFilterNavigationItemViewModelFactory>();

        // List
        services.AddTransient<AvatarListItemViewModelFactory>();

        return services;
    }
}