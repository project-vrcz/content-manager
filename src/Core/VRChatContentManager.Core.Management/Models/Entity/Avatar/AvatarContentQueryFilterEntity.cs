using FreeSql.DataAnnotations;

#pragma warning disable CS8618

namespace VRChatContentManager.Core.Management.Models.Entity.Avatar;

[Table(Name = "avatar_content_query_filters")]
public sealed class AvatarContentQueryFilterEntity
{
    [Column(IsPrimary = true)] public Guid Id { get; set; }
    public string Name { get; set; }
}