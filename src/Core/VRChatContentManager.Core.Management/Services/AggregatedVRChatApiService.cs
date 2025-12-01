    using VRChatContentManager.Core.Models.VRChatApi.Rest.Avatars;
using VRChatContentManager.Core.Services.UserSession;
using VRChatContentManager.Core.Services.VRChatApi;

namespace VRChatContentManager.Core.Management.Services;

public sealed class AggregatedVRChatApiService(UserSessionManagerService userSessionManagerService)
{
    public async ValueTask<VRChatApiAvatar[]> GetAllOwnAvatarsAsync()
    {
        var allAvatars = new List<VRChatApiAvatar>();

        foreach (var apiClient in GetApiClients())
        {
            var avatars = await apiClient.GetAllOwnAvatarsAsync();
            allAvatars.AddRange(avatars);
        }

        return allAvatars.ToArray();
    }

    public VRChatApiClient[] GetApiClients()
    {
        return userSessionManagerService.Sessions
            .Where(session => session.State == UserSessionState.LoggedIn)
            .Where(session => session.TryGetSessionScope() is not null)
            .Select(session => session.GetApiClient())
            .ToArray();
    }
}