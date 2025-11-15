using SqlSugar;
using VRChatContentManager.Core.Management.Models.Entity.Avatar;

namespace VRChatContentManager.Core.Management.Services;

public sealed class AvatarContentManagementService(SqlSugarClient sqlSugarClient)
{
    public async ValueTask<List<AvatarContentEntity>> GetAllAvatarsAsync()
    {
        return await sqlSugarClient
            .Queryable<AvatarContentEntity>()
            .Includes(e => e.LocalTags)
            .ToListAsync();
    }

    public async ValueTask<List<AvatarContentEntity>> GetAvatarsByIdsAsync(string[] ids)
    {
        return await sqlSugarClient
            .Queryable<AvatarContentEntity>()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();
    }

    public async ValueTask<List<AvatarContentTagEntity>> GetAllTagsAsync()
    {
        return await sqlSugarClient
            .Queryable<AvatarContentTagEntity>()
            .ToListAsync();
    }
}