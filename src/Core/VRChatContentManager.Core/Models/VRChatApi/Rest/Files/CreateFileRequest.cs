using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Files;

public record CreateFileRequest(
    [property: JsonPropertyName("name")] string FileName,
    [property: JsonPropertyName("mimeType")]
    string MimeType,
    [property: JsonPropertyName("extension")]
    string Extension
);