using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Avatars;

public record CreateAvatarVersionRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("assetUrl")]
    string AssetUrl,
    [property: JsonPropertyName("assetVersion")]
    int AssetVersion,
    [property: JsonPropertyName("platform")]
    string Platform,
    [property: JsonPropertyName("unityVersion")]
    string UnityVersion,
    [property: JsonPropertyName("imageUrl")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? ThumbnailUrl = null,
    [property: JsonPropertyName("description")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Description = null,
    [property: JsonPropertyName("tags")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string[]? Tags = null,
    [property: JsonPropertyName("releaseStatus")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? ReleaseStatus = null
);