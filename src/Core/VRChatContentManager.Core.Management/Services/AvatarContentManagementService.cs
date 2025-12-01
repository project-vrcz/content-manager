using FreeSql;
using VRChatContentManager.Core.Management.Mappers;
using VRChatContentManager.Core.Management.Models.Entity.Avatar;

namespace VRChatContentManager.Core.Management.Services;

public sealed class AvatarContentManagementService(
    IFreeSql freeSql,
    AggregatedVRChatApiService aggregatedApiService)
{
    private readonly IBaseRepository<AvatarContentEntity> _avatarRepository =
        freeSql.GetAggregateRootRepository<AvatarContentEntity>();

    public async ValueTask RefreshAllAvatarsAsync()
    {
        var supportedPlatformEntities = await GetAllSupportedPlatformsAsync();
        var avatars = await aggregatedApiService.GetAllOwnAvatarsAsync();

        var allSupportedPlatform = avatars.SelectMany(avatar => avatar.UnityPackages)
            .Where(package => package.Variant == "security")
            .Select(package => package.Platform)
            .Distinct()
            .ToArray();

        foreach (var platform in allSupportedPlatform)
        {
            if (supportedPlatformEntities.Any(e => e.Platform == platform))
                continue;

            var entity = await AddSupportedPlatformAsync(platform);
            supportedPlatformEntities.Add(entity);
        }

        foreach (var avatar in avatars)
        {
            var avatarSupportedPlatforms = avatar.UnityPackages
                .Where(package => package.Variant == "security")
                .Select(package => package.Platform)
                .Distinct()
                .ToArray();

            var avatarEntity = AvatarMapper.ToAvatarContentEntity(avatar);
            avatarEntity.SupportedPlatform = supportedPlatformEntities
                .Where(platform => avatarSupportedPlatforms.Contains(platform.Platform))
                .ToList();

            var existingEntity = await freeSql
                .Select<AvatarContentEntity>()
                .Where(e => e.Id == avatarEntity.Id)
                .IncludeMany(a => a.LocalTags)
                .ToOneAsync();

            if (existingEntity is not null)
                avatarEntity.LocalTags = existingEntity.LocalTags;

            await freeSql.InsertOrUpdate<AvatarContentEntity>()
                .SetSource(avatarEntity)
                .ExecuteAffrowsAsync();
        }
    }

    public async ValueTask<List<AvatarContentEntity>> GetAllAvatarsAsync()
    {
        return await _avatarRepository.Select
            .IncludeMany(avatar => avatar.LocalTags)
            .IncludeMany(avatar => avatar.SupportedPlatform)
            .ToListAsync();
    }

    public async ValueTask<List<AvatarContentEntity>> GetAvatarsByIdsAsync(string[] ids)
    {
        return await freeSql
            .Select<AvatarContentEntity>()
            .Where(e => ids.Contains(e.Id))
            .IncludeMany(avatar => avatar.LocalTags)
            .IncludeMany(avatar => avatar.SupportedPlatform)
            .ToListAsync();
    }

    public async ValueTask UpdateAvatarAsync(string id, Action<AvatarContentEntity> action)
    {
        var entity = await _avatarRepository.Select
            .Where(e => e.Id == id)
            .FirstAsync<AvatarContentEntity>();
        
        if (entity is null)
            throw new InvalidOperationException("Avatar not found");
        
        action(entity);

        await _avatarRepository.UpdateAsync(entity);
    }

    public async ValueTask<List<AvatarContentQueryFilterEntity>> GetAllQueryFiltersAsync()
    {
        return await freeSql
            .Select<AvatarContentQueryFilterEntity>()
            .ToListAsync();
    }

    public async ValueTask<List<AvatarContentTagEntity>> GetAllTagsAsync()
    {
        return await freeSql
            .Select<AvatarContentTagEntity>()
            .ToListAsync();
    }

    public async ValueTask<AvatarContentTagEntity> AddTagAsync(string tag)
    {
        var entity = new AvatarContentTagEntity
        {
            Tag = tag
        };

        await freeSql.Insert(entity).ExecuteAffrowsAsync();
        return entity;
    }

    public async ValueTask<List<AvatarContentSupportedPlatformEntity>> GetAllSupportedPlatformsAsync()
    {
        return await freeSql
            .Select<AvatarContentSupportedPlatformEntity>()
            .ToListAsync();
    }

    private async ValueTask<AvatarContentSupportedPlatformEntity> AddSupportedPlatformAsync(string platform)
    {
        var entity = new AvatarContentSupportedPlatformEntity
        {
            Platform = platform
        };

        await freeSql.Insert(entity).ExecuteAffrowsAsync();
        return entity;
    }
}