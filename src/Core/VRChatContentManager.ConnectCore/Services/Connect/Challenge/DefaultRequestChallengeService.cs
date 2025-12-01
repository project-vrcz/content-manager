namespace VRChatContentManager.ConnectCore.Services.Connect.Challenge;

public sealed class DefaultRequestChallengeService : IRequestChallengeService
{
    public Task RequestChallengeAsync(string code, string clientId, string identityPrompt, string clientName)
    {
        return Task.CompletedTask;
    }

    public Task CompleteChallengeAsync(string clientId)
    {
        return Task.CompletedTask;
    }
}