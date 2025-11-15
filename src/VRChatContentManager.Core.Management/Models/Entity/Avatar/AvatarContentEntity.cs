using SqlSugar;

// https://www.donet5.com/Home/Doc?typeId=1182
#pragma warning disable CS8618

namespace VRChatContentManager.Core.Management.Models.Entity.Avatar;

[SugarTable("avatars")]
public sealed class AvatarContentEntity
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; } = "";
    public string ThumbnailImageUrl { get; set; }
    public string AuthorId { get; set; }
    public string ReleaseStatus { get; set; }

    [Navigate(typeof(AvatarContentSupportedPlatformMappingEntity),
        nameof(AvatarContentSupportedPlatformMappingEntity.AvatarContentEntityId),
        nameof(AvatarContentSupportedPlatformMappingEntity.AvatarContentSupportedPlatformEntityId))]
    public List<AvatarContentSupportedPlatformEntity> SupportedPlatform { get; set; }

    [Navigate(typeof(AvatarContentTagMappingEntity),
        nameof(AvatarContentTagMappingEntity.AvatarContentEntityId),
        nameof(AvatarContentTagMappingEntity.AvatarContentTagEntityId))]
    public List<AvatarContentTagEntity> LocalTags { get; set; }
}