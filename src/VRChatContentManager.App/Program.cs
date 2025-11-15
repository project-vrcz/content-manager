using Avalonia;
using System;
using System.IO;
using System.Runtime.Versioning;
using HotAvalonia;
using Lemon.Hosting.AvaloniauiDesktop;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using VRChatContentManager.App.Extensions;
using VRChatContentManager.Core.Extensions;
using VRChatContentManager.Core.Management.Extensions;
using VRChatContentManager.Core.Services.App;

namespace VRChatContentManager.App;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static void Main(string[] args)
    {
        var builder = new HostApplicationBuilder();

        var jsonLogPath = Path.Combine(AppStorageService.GetStoragePath(), "logs", "log-.json");
        var plainTextLogPath = Path.Combine(AppStorageService.GetStoragePath(), "logs", "log-.log");
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(applyThemeToRedirectedOutput: true, theme: AnsiConsoleTheme.Code)
            .WriteTo.Async(writer =>
                writer.File(new CompactJsonFormatter(), jsonLogPath,
                    rollingInterval: RollingInterval.Day))
            .WriteTo.Async(writer => 
                writer.File(plainTextLogPath, rollingInterval: RollingInterval.Day))
            .WriteTo.Debug()
            .CreateLogger();

        builder.Services.AddSerilog();

        builder.UseAppCore();
        builder.Services.AddManagementCoreServices();
        builder.Services.AddAppServices();
        builder.Services.AddAvaloniauiDesktopApplication<App>(appBuilder => appBuilder
            .UseHotReload()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace());

        var app = builder.Build();

        try
        {
            app.RunAvaloniauiApplication(args);
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Oops, the application has crashed!");
            Environment.ExitCode = -1;
        }
        finally
        {
            app.Dispose();
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseHotReload()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}