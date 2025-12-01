using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Files;

public record CompleteFileUploadRequest(
    [property: JsonPropertyName("etags")] string[] ETags
);