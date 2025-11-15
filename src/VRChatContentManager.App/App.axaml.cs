using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Net.Http;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.Dialogs;
using VRChatContentManager.App.Pages;
using VRChatContentManager.App.Pages.ContentManager;
using VRChatContentManager.App.Pages.ContentManager.Avatar;
using VRChatContentManager.App.Pages.GettingStarted;
using VRChatContentManager.App.Pages.HomeTab;
using VRChatContentManager.App.Pages.Settings;
using VRChatContentManager.App.ViewModels;
using VRChatContentManager.App.ViewModels.ContentManager;
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
using VRChatContentManager.App.Views.ContentManager;
using VRChatContentManager.App.Views.Data.Connect;
using VRChatContentManager.App.Views.Data.PublishTasks;
using VRChatContentManager.App.Views.Data.Settings;
using VRChatContentManager.App.Views.Settings;
using VRChatContentManager.Core;
using VRChatContentManager.Core.Services.App;
using AddAccountPageViewModel = VRChatContentManager.App.ViewModels.Pages.AddAccountPageViewModel;

namespace VRChatContentManager.App;

public partial class App : Application
{
#pragma warning disable CS8600
#pragma warning disable CS8603
    public new static App Current => (App)Application.Current;
#pragma warning restore CS8603
#pragma warning restore CS8600

    private readonly IServiceProvider _serviceProvider = null!;

    public readonly AppWebImageLoader AsyncImageLoader;

    public App()
    {
        // Make Previewer happy
        var httpClient = new HttpClient();
        httpClient.AddUserAgent();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        AsyncImageLoader = new AppWebImageLoader(new RemoteImageService(httpClient, memoryCache), memoryCache);
    }

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        AsyncImageLoader = _serviceProvider.GetRequiredService<AppWebImageLoader>();
    }

    public override void Initialize()
    {
        ViewLocator.Register<BootstrapPageViewModel, BootstrapPage>();

        ViewLocator.Register<HomePageViewModel, HomePage>();
        ViewLocator.Register<SettingsPageViewModel, SettingsPage>();

        // HomePage Tabs
        ViewLocator.Register<HomeTasksPageViewModel, HomeTasksPage>();
        ViewLocator.Register<HomeContentsPageViewModel, HomeContentsPage>();

        // Getting Started Pages
        ViewLocator.Register<GuideWelcomePageViewModel, GuideWelcomePage>();
        ViewLocator.Register<GuideSetupUnityPageViewModel, GuideSetupUnityPage>();
        ViewLocator.Register<GuideFinishSetupPageViewModel, GuideFinishPage>();

        // Settings Pages
        ViewLocator.Register<SettingsFixAccountPageViewModel, SettingsFixAccountPage>();
        ViewLocator.Register<AddAccountPageViewModel, AddAccountPage>();

        // Dialogs
        ViewLocator.Register<TwoFactorAuthDialogViewModel, TwoFactorAuthDialog>();
        ViewLocator.Register<RequestChallengeDialogViewModel, RequestChallengeDialog>();

        // Data
        ViewLocator.Register<PublishTaskManagerViewModel, PublishTaskManagerView>();
        ViewLocator.Register<InvalidSessionTaskManagerViewModel, InvalidSessionTaskManagerView>();
        ViewLocator.Register<PublishTaskViewModel, PublishTaskView>();

        ViewLocator.Register<RpcClientSessionViewModel, RpcClientSessionView>();
        ViewLocator.Register<UserSessionViewModel, UserSessionView>();

        // Settings Section
        ViewLocator.Register<ConnectSettingsViewModel, ConnectSettingsView>();
        ViewLocator.Register<AccountsSettingsViewModel, AccountsSettingsView>();
        ViewLocator.Register<SessionsSettingsViewModel, SessionsSettingsView>();
        ViewLocator.Register<AboutSettingsViewModel, AboutSettingsView>();

        // Content Manager
        ViewLocator.Register<ContentManagerViewModel, ContentManagerMainView>();
        ViewLocator.Register<ContentManagerHomePageViewModel, ContentManagerHomePage>();
        ViewLocator.Register<ContentManagerAvatarRootPageViewModel, ContentManagerAvatarRootPage>();

        AvaloniaXamlLoader.Load(this);

        this.AttachDeveloperTools();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void ShowWindowClicked(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        desktop.MainWindow?.Show();
        desktop.MainWindow?.Activate();
    }

    private void ExitAppClicked(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        desktop.Shutdown();
    }
}