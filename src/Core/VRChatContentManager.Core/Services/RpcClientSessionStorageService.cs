using VRChatContentManager.ConnectCore.Models.ClientSession;
using VRChatContentManager.ConnectCore.Services.Connect.SessionStorage;
using VRChatContentManager.Core.Settings;
using VRChatContentManager.Core.Settings.Models;

namespace VRChatContentManager.Core.Services;

public sealed class RpcClientSessionStorageService(IWritableOptions<RpcSessionStorage> sessionsStorage)
    : ISessionStorageService
{
    public event EventHandler? SessionsChanged;

    public List<RpcClientSession> GetAllSessions()
    {
        return [..sessionsStorage.Value.Sessions];
    }

    public RpcClientSession? GetSessionByClientId(string clientId)
    {
        return sessionsStorage.Value.Sessions.Find(session => session.ClientId == clientId);
    }

    public async ValueTask AddSessionAsync(RpcClientSession session)
    {
        await sessionsStorage.UpdateAsync(sessions => { sessions.Sessions.Add(session); });

        SessionsChanged?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask RemoveSessionByClientIdAsync(string clientId)
    {
        await sessionsStorage.UpdateAsync(sessions =>
        {
            sessions.Sessions.RemoveAll(session => session.ClientId == clientId);
        });

        SessionsChanged?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask RemoveExpiredSessionsAsync()
    {
        await sessionsStorage.UpdateAsync(sessions =>
        {
            var now = DateTimeOffset.UtcNow;
            sessions.Sessions.RemoveAll(session => session.Expires < now);
        });

        SessionsChanged?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask<string> GetIssuerAsync()
    {
        var issuer = sessionsStorage.Value.IssuerKey;
        if (issuer is null)
        {
            issuer = Guid.CreateVersion7().ToString("D");
            await sessionsStorage.UpdateAsync(storage => { storage.IssuerKey = issuer; });
        }

        return issuer;
    }
}