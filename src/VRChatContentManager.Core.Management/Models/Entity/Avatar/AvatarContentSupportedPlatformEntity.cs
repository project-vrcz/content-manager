using SqlSugar;

// https://www.donet5.com/Home/Doc?typeId=1182
#pragma warning disable CS8618

namespace VRChatContentManager.Core.Management.Models.Entity.Avatar;

[SugarTable("avatar_content_supported_platforms")]
[SugarIndex("unique_avatar_content_supported_platforms_platform", nameof(Platform), OrderByType.Asc, true)]
public sealed class AvatarContentSupportedPlatformEntity
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public string Platform { get; set; }

    [Navigate(typeof(AvatarContentSupportedPlatformMappingEntity),
        nameof(AvatarContentSupportedPlatformMappingEntity.AvatarContentSupportedPlatformEntityId),
        nameof(AvatarContentSupportedPlatformMappingEntity.AvatarContentEntityId))]
    public List<AvatarContentEntity> Entities { get; set; }
}

public sealed class AvatarContentSupportedPlatformMappingEntity
{
    [SugarColumn(IsPrimaryKey = true)] public int AvatarContentEntityId { get; set; }
    [SugarColumn(IsPrimaryKey = true)] public int AvatarContentSupportedPlatformEntityId { get; set; }
}