using System.Text.Json.Serialization;
using VRChatContentManager.Core.Models.VRChatApi.Rest.UnityPackages;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Worlds;

public record VRChatApiWorld(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("unityPackages")] VRChatApiUnityPackage[] UnityPackages,
    [property: JsonPropertyName("authorId")] string AuthorId,
    [property: JsonPropertyName("imageUrl")] string? ImageUrl
);