using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Auth;

public record VerifyTotpRequest(
    [property: JsonPropertyName("code")] string Code
);