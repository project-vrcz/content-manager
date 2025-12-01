using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.ContentManagement.Extensions;
using VRChatContentManager.App.Extensions;
using VRChatContentManager.App.Shared;
using VRChatContentManager.App.ViewModels;
using VRChatContentManager.App.Views;

namespace VRChatContentManager.App;

public partial class App : Application
{
#pragma warning disable CS8600
#pragma warning disable CS8603
    public new static App Current => (App)Application.Current;
#pragma warning restore CS8603
#pragma warning restore CS8600

    private readonly IServiceProvider _serviceProvider = null!;

    public App()
    {
    }

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        AppWebImageLoader.Instance = _serviceProvider.GetRequiredService<AppWebImageLoader>();
    }

    public override void Initialize()
    {
        ViewLocator.RegisterAppViews();
        ViewLocator.RegisterContentManagementViews();

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