using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.ViewModels;
using VRChatContentManager.App.ViewModels.Data;
using VRChatContentManager.App.ViewModels.Data.Connect;
using VRChatContentManager.App.ViewModels.Data.PublishTasks;
using VRChatContentManager.App.ViewModels.Dialogs;
using VRChatContentManager.App.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Pages.GettingStarted;
using VRChatContentManager.App.ViewModels.Pages.HomeTab;
using VRChatContentManager.App.ViewModels.Pages.Settings;
using VRChatContentManager.App.ViewModels.Settings;
using VRChatContentManager.ConnectCore.Services.Connect.Challenge;

namespace VRChatContentManager.App.Extensions;

public static class ServicesExtenstion
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        // Dialogs
        services.AddTransient<TwoFactorAuthDialogViewModelFactory>();
        services.AddTransient<RequestChallengeDialogViewModelFactory>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();

        services.AddTransient<BootstrapPageViewModel>();

        services.AddTransient<HomePageViewModel>();
        services.AddTransient<SettingsPageViewModel>();

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