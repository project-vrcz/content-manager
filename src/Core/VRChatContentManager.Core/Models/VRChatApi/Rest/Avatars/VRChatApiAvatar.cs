using System.Text.Json.Serialization;
using VRChatContentManager.Core.Models.VRChatApi.Rest.UnityPackages;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Avatars;

public record VRChatApiAvatar(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("unityPackages")]
    VRChatApiUnityPackage[] UnityPackages,
    [property: JsonPropertyName("authorId")]
    string AuthorId,
    [property: JsonPropertyName("releaseStatus")]
    string ReleaseStatus = "private",
    [property: JsonPropertyName("imageUrl")]
    string? ImageUrl = null,
    [property: JsonPropertyName("thumbnailImageUrl")]
    string? ThumbnailImageUrl = null
);