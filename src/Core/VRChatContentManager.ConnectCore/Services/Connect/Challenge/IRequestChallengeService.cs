namespace VRChatContentManager.ConnectCore.Services.Connect.Challenge;

public interface IRequestChallengeService
{
    Task RequestChallengeAsync(string code, string clientId, string identityPrompt, string clientName);
    Task CompleteChallengeAsync(string clientId);
}