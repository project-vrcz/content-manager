using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.UnityPackages;

public record VRChatApiUnityPackage
(
    [property: JsonPropertyName("assetUrl")] string AssetUrl,
    [property: JsonPropertyName("assetVersion")] int AssetVersion,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("unityVersion")] string UnityVersion,
    [property: JsonPropertyName("variant")] string? Variant = null
);