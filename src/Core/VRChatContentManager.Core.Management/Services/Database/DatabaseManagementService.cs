using VRChatContentManager.Core.Management.Models.Entity.Avatar;

namespace VRChatContentManager.Core.Management.Services.Database;

public sealed class DatabaseManagementService(IFreeSql freeSql)
{
    public void InitializeDatabase()
    {
        freeSql.CodeFirst.SyncStructure<AvatarContentEntity>();
        freeSql.CodeFirst.SyncStructure<AvatarContentTagEntity>();
        freeSql.CodeFirst.SyncStructure<AvatarContentTagMappingEntity>();

        freeSql.CodeFirst.SyncStructure<AvatarContentSupportedPlatformEntity>();
        freeSql.CodeFirst.SyncStructure<AvatarContentSupportedPlatformMappingEntity>();

        freeSql.CodeFirst.SyncStructure<AvatarContentQueryFilterEntity>();
    }
}