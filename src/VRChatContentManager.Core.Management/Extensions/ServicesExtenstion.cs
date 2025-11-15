using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;
using VRChatContentManager.Core.Management.Services;
using VRChatContentManager.Core.Management.Services.Database;
using VRChatContentManager.Core.Services.App;

namespace VRChatContentManager.Core.Management.Extensions;

public static class ServicesExtenstion
{
    public static IServiceCollection AddManagementCoreServices(this IServiceCollection services)
    {
        // Have no choice
        StaticConfig.EnableAot = true;

        services.AddTransient<SqlSugarClient>(serviceProvider =>
        {
            var dbPath = Path.Combine(AppStorageService.GetStoragePath(), "content-manager.db");
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = dbPath
            };

            var connectionString = connectionStringBuilder.ToString();

            var logger = serviceProvider.GetRequiredService<ILogger<SqlSugarClient>>();

            return new SqlSugarClient(new ConnectionConfig
            {
                IsAutoCloseConnection = true,
                DbType = DbType.Sqlite,
                ConnectionString = connectionString,
            }, db =>
            {
                db.Aop.OnLogExecuting = (sql, parameters) =>
                {
                    var fullSql = UtilMethods.GetNativeSql(sql, parameters);
                    logger.LogInformation("Executing SQL: {Sql}", fullSql);
                };
                db.Aop.OnError = exception => { logger.LogError(exception, "SQL Error: {Sql}", exception.Sql); };
            });
        });

        services.AddTransient<DatabaseManagementService>();
        services.AddHostedService<DatabaseManagementHostedService>();

        services.AddTransient<AvatarContentManagementService>();

        return services;
    }
}