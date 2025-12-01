using FreeSql.DataAnnotations;

#pragma warning disable CS8618

namespace VRChatContentManager.Core.Management.Models.Entity.Avatar;

[Table(Name = "avatars")]
public sealed class AvatarContentEntity
{
    [Column(IsPrimary = true)] public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; } = "";
    public string? ThumbnailImageUrl { get; set; }
    public string? ImageUrl { get; set; }

    public string AuthorId { get; set; }
    public string ReleaseStatus { get; set; }

    [Navigate(ManyToMany = typeof(AvatarContentSupportedPlatformMappingEntity))]
    public List<AvatarContentSupportedPlatformEntity> SupportedPlatform { get; set; }

    [Navigate(ManyToMany = typeof(AvatarContentTagMappingEntity))]
    public List<AvatarContentTagEntity> LocalTags { get; set; }
}