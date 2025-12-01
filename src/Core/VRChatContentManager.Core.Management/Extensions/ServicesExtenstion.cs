using FreeSql;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VRChatContentManager.Core.Management.Services;
using VRChatContentManager.Core.Management.Services.Database;
using VRChatContentManager.Core.Services.App;

namespace VRChatContentManager.Core.Management.Extensions;

public static class ServicesExtenstion
{
    public static IServiceCollection AddManagementCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IFreeSql>(serviceProvider =>
        {
            var dbPath = Path.Combine(AppStorageService.GetStoragePath(), "content-manager.db");
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = dbPath
            };

            var freeSql = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, connectionStringBuilder.ToString())
                .UseAdoConnectionPool(true)
                .UseMonitorCommand(command =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<IFreeSql>>();
                    logger.LogInformation("Executing SQL: {Sql}", command.CommandText);
                })
                .Build();

            return freeSql;
        });

        services.AddTransient<DatabaseManagementService>();
        services.AddHostedService<DatabaseManagementHostedService>();

        services.AddTransient<AggregatedVRChatApiService>();

        services.AddTransient<AvatarContentManagementService>();

        return services;
    }
}