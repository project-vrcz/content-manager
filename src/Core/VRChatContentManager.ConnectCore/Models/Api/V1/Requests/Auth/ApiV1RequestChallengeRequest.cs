namespace VRChatContentManager.ConnectCore.Models.Api.V1.Requests.Auth;

public class ApiV1RequestChallengeRequest
{
    public required string ClientId { get; set; }
    public required string IdentityPrompt { get; set; }
    public required string ClientName { get; set; }
}