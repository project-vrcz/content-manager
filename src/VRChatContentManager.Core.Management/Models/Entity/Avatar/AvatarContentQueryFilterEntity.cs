using SqlSugar;

// https://www.donet5.com/Home/Doc?typeId=1182
#pragma warning disable CS8618

namespace VRChatContentManager.Core.Management.Models.Entity.Avatar;

[SugarTable("avatar_content_query_filters")]
public sealed class AvatarContentQueryFilterEntity
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string Name { get; set; }
}