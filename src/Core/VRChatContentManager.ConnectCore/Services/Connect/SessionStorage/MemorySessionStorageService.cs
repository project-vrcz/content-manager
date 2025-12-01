using VRChatContentManager.ConnectCore.Models.ClientSession;

namespace VRChatContentManager.ConnectCore.Services.Connect.SessionStorage;

public sealed class MemorySessionStorageService : ISessionStorageService
{
    private readonly Lock _sessionLock = new();

    private readonly List<RpcClientSession> _sessions = [];

    private readonly string _sessionIssuer = Guid.NewGuid().ToString("D");

    public event EventHandler? SessionsChanged;

    public List<RpcClientSession> GetAllSessions()
    {
        lock (_sessionLock)
        {
            return [.._sessions];
        }
    }

    public RpcClientSession? GetSessionByClientId(string clientId)
    {
        lock (_sessionLock)
        {
            return _sessions.Find(session => session.ClientId == clientId);
        }
    }

    public ValueTask AddSessionAsync(RpcClientSession session)
    {
        lock (_sessionLock)
        {
            _sessions.Add(session);
        }

        SessionsChanged?.Invoke(this, EventArgs.Empty);

        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveSessionByClientIdAsync(string clientId)
    {
        lock (_sessionLock)
        {
            _sessions.RemoveAll(session => session.ClientId == clientId);
        }

        SessionsChanged?.Invoke(this, EventArgs.Empty);

        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveExpiredSessionsAsync()
    {
        var now = DateTimeOffset.UtcNow;
        lock (_sessionLock)
        {
            _sessions.RemoveAll(session => session.Expires <= now);
        }

        SessionsChanged?.Invoke(this, EventArgs.Empty);

        return ValueTask.CompletedTask;
    }

    public ValueTask<string> GetIssuerAsync() => ValueTask.FromResult(_sessionIssuer);
}