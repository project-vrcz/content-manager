using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using VRChatContentManager.ConnectCore.Extensions;
using VRChatContentManager.ConnectCore.Services;
using VRChatContentManager.ConnectCore.Services.Connect.Metadata;
using VRChatContentManager.ConnectCore.Services.Connect.SessionStorage;
using VRChatContentManager.ConnectCore.Services.PublishTask;
using VRChatContentManager.Core.Services;
using VRChatContentManager.Core.Services.App;
using VRChatContentManager.Core.Services.PublishTask;
using VRChatContentManager.Core.Services.PublishTask.BundleProcesser;
using VRChatContentManager.Core.Services.PublishTask.Connect;
using VRChatContentManager.Core.Services.PublishTask.ContentPublisher;
using VRChatContentManager.Core.Services.UserSession;
using VRChatContentManager.Core.Services.VRChatApi;
using VRChatContentManager.Core.Services.VRChatApi.S3;
using VRChatContentManager.Core.Settings;
using VRChatContentManager.Core.Settings.Models;

namespace VRChatContentManager.Core.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddAppCore(this IServiceCollection services)
    {
        services.AddConnectCore();
        services.AddSingleton<ISessionStorageService, RpcClientSessionStorageService>();
        services.AddSingleton<ITokenSecretKeyProvider, RpcTokenSecretKeyProvider>();
        services.AddSingleton<IFileService, TempFileService>();
        services.AddTransient<IConnectMetadataProvider, ConnectMetadataProvider>();

        // Connect Publish Service
        services.AddTransient<IWorldPublishTaskService, WorldPublishTaskService>();
        services.AddTransient<WorldContentPublisherFactory>();
        
        services.AddTransient<IAvatarPublishTaskService, AvatarPublishTaskService>();
        services.AddTransient<AvatarContentPublisherFactory>();

        services.AddTransient<BundleCompressProcesser>();
        
        services.AddMemoryCache();

        services.AddTransient<ConcurrentMultipartUploaderFactory>();

        services.AddSingleton<RemoteImageService>();
        services.AddHttpClient<RemoteImageService>(client => { client.AddUserAgent(); });

        services.AddTransient<VRChatApiClientFactory>();
        
        services.AddSingleton<UserSessionManagerService>();

        services.AddScoped<UserSessionScopeService>();
        services.AddScoped<TaskManagerService>();

        services.AddTransient<UserSessionFactory>();
        services.AddTransient<ContentPublishTaskFactory>();

        // HttpClient only use for upload content to aws s3, DO NOT USE FOR OTHER REQUESTS UNLESS YOU WANT TO LEAK CREDENTIALS
        services.AddHttpClient<ContentPublishTaskFactory>(client =>
            {
                client.AddUserAgent();
                client.Timeout = Timeout.InfiniteTimeSpan;
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                UseCookies = false,
                MaxConnectionsPerServer = 10,
                PooledConnectionLifetime = TimeSpan.Zero,
                EnableMultipleHttp2Connections = true,
                EnableMultipleHttp3Connections = true,
                ConnectTimeout = TimeSpan.FromSeconds(5)
            })
            .AddResilienceHandler("awsClient", builder =>
            {
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    UseJitter = true,
                    ShouldRetryAfterHeader = true,
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Linear
                });
            });

        return services;
    }

    public static IHostApplicationBuilder UseAppCore(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAppCore();
        
        const string sessionsFileName = "sessions.json";
        builder.Configuration.AddAppJsonFile(sessionsFileName);
        
        var sessionsSection = builder.Configuration.GetSection("Sessions");
        builder.Services.Configure<UserSessionStorage>(sessionsSection);
        builder.Services.AddWriteableOptions<UserSessionStorage>(sessionsSection.Key, sessionsFileName);
        
        const string appSettingsFileName = "settings.json";
        builder.Configuration.AddAppJsonFile(appSettingsFileName);
        
        var appSettingsSection = builder.Configuration.GetSection("Settings");
        builder.Services.Configure<AppSettings>(appSettingsSection);
        builder.Services.AddWriteableOptions<AppSettings>(appSettingsSection.Key, appSettingsFileName);
        
        const string rpcSessionsFileName = "rpc-sessions.json";
        builder.Configuration.AddAppJsonFile(rpcSessionsFileName);
        
        var rpcSessionsSection = builder.Configuration.GetSection("RpcSessions");
        builder.Services.Configure<RpcSessionStorage>(rpcSessionsSection);
        builder.Services.AddWriteableOptions<RpcSessionStorage>(rpcSessionsSection.Key, rpcSessionsFileName);

        return builder;
    }

    public static IServiceCollection AddWriteableOptions<T>(this IServiceCollection services, string sectionName,
        string filePath, bool useStoragePath = true)
        where T : class, new()
    {
        services.AddTransient<IWritableOptions<T>>(provider =>
        {
            if (provider.GetRequiredService<IConfiguration>() is not IConfigurationRoot configuration)
                throw new InvalidOperationException("Configuration is not an IConfigurationRoot");
            
            filePath = useStoragePath ? Path.Combine(AppStorageService.GetStoragePath(), filePath) : filePath;

            var options = provider.GetRequiredService<IOptionsMonitor<T>>();
            var writer = new OptionsWriter(configuration, filePath);

            return new WritableOptions<T>(sectionName, writer, options);
        });

        return services;
    }
    
    public static IConfigurationManager AddAppJsonFile(this IConfigurationManager configurationManager, string fileName)
    {
        var appSettingsPath = Path.Combine(AppStorageService.GetStoragePath(), fileName);
        configurationManager.AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true);
        return configurationManager;
    }
}