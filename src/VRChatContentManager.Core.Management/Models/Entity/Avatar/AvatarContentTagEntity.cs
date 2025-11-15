using SqlSugar;

// https://www.donet5.com/Home/Doc?typeId=1182
#pragma warning disable CS8618

namespace VRChatContentManager.Core.Management.Models.Entity.Avatar;

[SugarTable("avatar_content_tags")]
[SugarIndex("unique_avatar_content_tags_tag", nameof(Tag), OrderByType.Asc, true)]
public sealed class AvatarContentTagEntity
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public string Tag { get; set; }

    [Navigate(typeof(AvatarContentTagMappingEntity),
        nameof(AvatarContentTagMappingEntity.AvatarContentTagEntityId),
        nameof(AvatarContentTagMappingEntity.AvatarContentEntityId))]
    public List<AvatarContentEntity> TagContents { get; set; }
}

public sealed class AvatarContentTagMappingEntity
{
    [SugarColumn(IsPrimaryKey = true)] public int AvatarContentEntityId { get; set; }
    [SugarColumn(IsPrimaryKey = true)] public int AvatarContentTagEntityId { get; set; }
}