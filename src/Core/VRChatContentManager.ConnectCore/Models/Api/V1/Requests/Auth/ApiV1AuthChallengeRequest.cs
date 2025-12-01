namespace VRChatContentManager.ConnectCore.Models.Api.V1.Requests.Auth;

public class ApiV1AuthChallengeRequest
{
    public required string Code { get; set; }
    public required string ClientId { get; set; }
    public required string IdentityPrompt { get; set; }
}