using SqlSugar;
using VRChatContentManager.Core.Management.Models.Entity.Avatar;

namespace VRChatContentManager.Core.Management.Services.Database;

public sealed class DatabaseManagementService(SqlSugarClient sqlSugarClient)
{
    public void InitializeDatabase()
    {
        sqlSugarClient.DbMaintenance.CreateDatabase();

        sqlSugarClient.CodeFirst.InitTables<AvatarContentEntity>();
        sqlSugarClient.CodeFirst.InitTables<AvatarContentTagEntity>();
        sqlSugarClient.CodeFirst.InitTables<AvatarContentTagMappingEntity>();
        sqlSugarClient.CodeFirst.InitTables<AvatarContentSupportedPlatformEntity>();
        sqlSugarClient.CodeFirst.InitTables<AvatarContentSupportedPlatformMappingEntity>();
        sqlSugarClient.CodeFirst.InitTables<AvatarContentQueryFilterEntity>();
    }
}