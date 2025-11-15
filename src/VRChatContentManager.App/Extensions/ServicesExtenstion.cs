using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.ViewModels;
using VRChatContentManager.App.ViewModels.ContentManager;
using VRChatContentManager.App.ViewModels.ContentManager.Data.Navigation;
using VRChatContentManager.App.ViewModels.ContentManager.Data.Navigation.Avatar;
using VRChatContentManager.App.ViewModels.ContentManager.Pages;
using VRChatContentManager.App.ViewModels.ContentManager.Pages.Avatar;
using VRChatContentManager.App.ViewModels.Data;
using VRChatContentManager.App.ViewModels.Data.Connect;
using VRChatContentManager.App.ViewModels.Data.PublishTasks;
using VRChatContentManager.App.ViewModels.Dialogs;
using VRChatContentManager.App.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Pages.GettingStarted;
using VRChatContentManager.App.ViewModels.Pages.HomeTab;
using VRChatContentManager.App.ViewModels.Pages.Settings;
using VRChatContentManager.App.ViewModels.Settings;
using VRChatContentManager.App.Views;
using VRChatContentManager.ConnectCore.Services.Connect.Challenge;
using AddAccountPageViewModel = VRChatContentManager.App.ViewModels.Pages.AddAccountPageViewModel;

namespace VRChatContentManager.App.Extensions;

public static class ServicesExtenstion
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<AppWebImageLoader>();

        services.AddSingleton<AppWindowService>();

        // Dialog
        services.AddSingleton<DialogService>();

        // Dialogs
        services.AddTransient<TwoFactorAuthDialogViewModelFactory>();
        services.AddTransient<RequestChallengeDialogViewModelFactory>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<NavigationService>();

        services.AddTransient<BootstrapPageViewModel>();

        services.AddTransient<HomePageViewModel>();
        services.AddTransient<SettingsPageViewModel>();

        #region Content Manager ViewModels

        // Content Manager ViewModels
        services.AddKeyedSingleton<NavigationService>(ServicesKeys.ContentManagerWindows);

        services.AddSingleton<ContentManagerWindowViewModel>();
        services.AddSingleton<ContentManagerWindow>();

        services.AddSingleton<ContentManagerViewModel>();

        // Content Manager Pages
        services.AddTransient<ContentManagerHomePageViewModel>();
        services.AddTransient<ContentManagerAvatarRootPageViewModel>();

        // Content Manager Navigation
        services.AddTransient<TreeNavigationItemViewModelFactory>();
        services.AddTransient<AvatarRootNavigationItemViewModel>();

        #endregion

        // Data ViewModels
        services.AddTransient<UserSessionViewModelFactory>();
        services.AddTransient<PublishTaskViewModelFactory>();
        services.AddTransient<PublishTaskManagerViewModelFactory>();

        services.AddTransient<RpcClientSessionViewModelFactory>();

        // HomePage Tabs
        services.AddTransient<HomeTasksPageViewModel>();
        services.AddTransient<HomeContentsPageViewModel>();

        // Getting Started Pages
        services.AddTransient<GuideWelcomePageViewModel>();
        services.AddTransient<GuideSetupUnityPageViewModel>();
        services.AddTransient<GuideFinishSetupPageViewModel>();

        // Settings Pages
        services.AddTransient<AddAccountPageViewModelFactory>();
        services.AddTransient<SettingsFixAccountPageViewModelFactory>();

        // Settings Sections
        services.AddTransient<AccountsSettingsViewModel>();
        services.AddTransient<ConnectSettingsViewModel>();
        services.AddTransient<SessionsSettingsViewModel>();
        services.AddTransient<AboutSettingsViewModel>();

        // Connect Core
        services.AddSingleton<IRequestChallengeService, RequestChallengeService>();

        return services;
    }
}