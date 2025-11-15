using Microsoft.Extensions.Hosting;

namespace VRChatContentManager.Core.Management.Services.Database;

public sealed class DatabaseManagementHostedService(DatabaseManagementService databaseManagementService)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        databaseManagementService.InitializeDatabase();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}