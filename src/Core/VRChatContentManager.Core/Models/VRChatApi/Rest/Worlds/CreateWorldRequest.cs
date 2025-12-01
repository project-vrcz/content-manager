using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Worlds;

public record CreateWorldRequest(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("assetUrl")]
    string AssetUrl,
    [property: JsonPropertyName("assetVersion")]
    int AssetVersion,
    [property: JsonPropertyName("platform")]
    string Platform,
    [property: JsonPropertyName("unityVersion")]
    string UnityVersion,
    [property: JsonPropertyName("worldSignature")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? WorldSignature,
    [property: JsonPropertyName("imageUrl")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? ImageUrl,
    [property: JsonPropertyName("description")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Description,
    [property: JsonPropertyName("tags")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string[]? Tags,
    [property: JsonPropertyName("releaseStatus")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? ReleaseStatus,
    [property: JsonPropertyName("capacity")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? Capacity,
    [property: JsonPropertyName("recommendedCapacity")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? RecommendedCapacity,
    [property: JsonPropertyName("previewYoutubeId")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? PreviewYoutubeId,
    [property: JsonPropertyName("udonProducts")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string[]? UdonProducts
);