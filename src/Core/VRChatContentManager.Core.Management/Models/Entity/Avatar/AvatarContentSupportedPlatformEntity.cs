using FreeSql.DataAnnotations;

#pragma warning disable CS8618

namespace VRChatContentManager.Core.Management.Models.Entity.Avatar;

[Table(Name = "avatar_content_supported_platforms")]
[Index("uk_platform", nameof(Platform), true)]
public sealed class AvatarContentSupportedPlatformEntity
{
    [Column(IsPrimary = true)] public Guid Id { get; set; } = Guid.CreateVersion7();

    public string Platform { get; set; }

    [Navigate(ManyToMany = typeof(AvatarContentSupportedPlatformMappingEntity))]
    public List<AvatarContentEntity> Avatars { get; set; }
}

public sealed class AvatarContentSupportedPlatformMappingEntity
{
    public string AvatarContentEntityId { get; set; }

    [Navigate(nameof(AvatarContentEntityId))]
    public AvatarContentEntity AvatarContentEntity { get; set; }

    public Guid AvatarContentSupportedPlatformEntityId { get; set; }

    [Navigate(nameof(AvatarContentSupportedPlatformEntityId))]
    public AvatarContentSupportedPlatformEntity AvatarContentSupportedPlatformEntity { get; set; }
}