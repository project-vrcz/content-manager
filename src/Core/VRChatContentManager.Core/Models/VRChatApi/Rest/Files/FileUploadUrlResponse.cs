using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Files;

public record FileUploadUrlResponse(
    [property: JsonPropertyName("url")] string Url
);