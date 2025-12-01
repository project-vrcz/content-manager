using Riok.Mapperly.Abstractions;
using VRChatContentManager.Core.Management.Models.Entity.Avatar;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Avatars;

namespace VRChatContentManager.Core.Management.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class AvatarMapper
{
    [MapperIgnoreTarget(nameof(AvatarContentEntity.LocalTags))]
    [MapperIgnoreTarget(nameof(AvatarContentEntity.SupportedPlatform))]
    public static partial AvatarContentEntity ToAvatarContentEntity(VRChatApiAvatar avatar);
}