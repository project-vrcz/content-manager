using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Auth;

public record RequireTwoFactorAuthResponse(
    [property: JsonPropertyName("requiresTwoFactorAuth")]
    Requires2FA[] RequiresTwoFactorAuth);