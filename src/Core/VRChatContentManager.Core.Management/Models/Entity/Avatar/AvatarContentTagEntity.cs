using FreeSql.DataAnnotations;

#pragma warning disable CS8618

namespace VRChatContentManager.Core.Management.Models.Entity.Avatar;

[Table(Name = "avatar_content_tags")]
[Index("uk_tag", nameof(Tag), true)]
public sealed class AvatarContentTagEntity
{
    [Column(IsPrimary = true)] public Guid Id { get; set; } = Guid.CreateVersion7();

    public string Tag { get; set; }

    [Navigate(ManyToMany = typeof(AvatarContentTagMappingEntity))]
    public List<AvatarContentEntity> Avatars { get; set; }
}

public sealed class AvatarContentTagMappingEntity
{
    public string AvatarContentEntityId { get; set; }

    [Navigate(nameof(AvatarContentEntityId))]
    public AvatarContentEntity AvatarContentEntity { get; set; }

    public Guid AvatarContentTagEntityId { get; set; }

    [Navigate(nameof(AvatarContentTagEntityId))]
    public AvatarContentTagEntity AvatarContentTagEntity { get; set; }
}