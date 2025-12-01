using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.Shared.Services;

namespace VRChatContentManager.App.Shared.Extensions;

public static class ServicesExtenstion
{
    public static IServiceCollection AddAppShared(this IServiceCollection services)
    {
        services.AddSingleton<AppWebImageLoader>();
        services.AddSingleton<AppWindowService>();
        services.AddSingleton<NavigationService>();

        // Dialog
        services.AddSingleton<DialogService>();

        return services;
    }
}