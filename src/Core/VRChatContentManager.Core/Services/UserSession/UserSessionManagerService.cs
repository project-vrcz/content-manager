using System.Net;
using Microsoft.Extensions.Logging;
using VRChatContentManager.Core.Settings;
using VRChatContentManager.Core.Settings.Models;

namespace VRChatContentManager.Core.Services.UserSession;

public sealed class UserSessionManagerService(
    UserSessionFactory sessionFactory,
    IWritableOptions<UserSessionStorage> userSessionStorage,
    ILogger<UserSessionManagerService> logger)
{
    private readonly List<UserSessionService> _sessions = [];
    public IReadOnlyList<UserSessionService> Sessions => _sessions.AsReadOnly();

    public event EventHandler<UserSessionService>? SessionCreated;
    public event EventHandler<UserSessionService>? SessionRemoved;

    public async Task RestoreSessionsAsync()
    {
        foreach (var (userId, sessionItem) in userSessionStorage.Value.Sessions)
        {
            var cookieContainer = new CookieContainer();
            foreach (var cookie in sessionItem.Cookies)
            {
                cookieContainer.Add(cookie);
            }

            var session = CreateOrGetSession(sessionItem.UserName, userId, cookieContainer);
            try
            {
                await session.CreateOrGetSessionScopeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to restore session for user ({UserId}) {UserName}", userId,
                    sessionItem.UserName);
            }
        }
    }

    public bool IsSessionExists(string userNameOrEmail)
    {
        return _sessions.Any(session =>
            session.UserNameOrEmail == userNameOrEmail ||
            session.CurrentUser?.UserName == userNameOrEmail
        );
    }

    public UserSessionService CreateOrGetSession(string userNameOrEmail, string? userId = null, CookieContainer?
        cookieContainer = null)
    {
        if (_sessions.FirstOrDefault(session =>
                (session.UserId is not null && userId == session.UserId) ||
                session.UserNameOrEmail == userNameOrEmail
            ) is { } existingSession)
        {
            return existingSession;
        }

        var session = sessionFactory.Create(userNameOrEmail, userId, cookieContainer,
            async (cookies, sessionUserId, userName) =>
            {
                if (sessionUserId is null || userName is null)
                    return;

                await userSessionStorage.UpdateAsync(sessions =>
                {
                    if (sessions.Sessions.TryGetValue(sessionUserId, out var session))
                    {
                        session.Cookies.Clear();
                        session.Cookies.AddRange(cookies.GetAllCookies());
                        return;
                    }

                    sessions.Sessions.Add(sessionUserId,
                        new UserSessionStorageItem(userName, new List<Cookie>(cookies.GetAllCookies())));
                });
            });

        _sessions.Add(session);
        OnSessionCreated(session);

        return session;
    }

    public async ValueTask<UserSessionService> HandleSessionAfterLogin(UserSessionService session)
    {
        if (Sessions.All(s => s != session))
            throw new InvalidOperationException("Session no exists in manager.");

        var user = await session.GetCurrentUserAsync();
        // Replace existing session if try login with same user
        if (Sessions.FirstOrDefault(existSession =>
                existSession != session && existSession.UserId == user.Id)
            is not { } existingSession)
            return session;

        logger.LogWarning("Replacing existing session for user ({UserId}) {UserName}", user.Id, user.UserName);
        logger.LogInformation("Logging out existing session for user ({UserId}) {UserName}", existingSession.UserId,
            existingSession.UserNameOrEmail);
        try
        {
            await existingSession.LogoutAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to logout existing session for user ({UserId}) {UserName}",
                existingSession.UserId, existingSession.UserNameOrEmail);
        }

        logger.LogInformation("Transferring cookies to existing session for user ({UserId}) {UserName}",
            existingSession.UserId, existingSession.UserNameOrEmail);
        foreach (Cookie cookie in existingSession.CookieContainer.GetAllCookies())
        {
            cookie.Expired = true;
        }

        var cookies = session.CookieContainer.GetAllCookies();
        foreach (Cookie cookie in cookies)
        {
            existingSession.CookieContainer.Add(cookie);
        }

        logger.LogInformation("Refreshing existing session for user ({UserId}) {UserName}", existingSession.UserId,
            existingSession.UserNameOrEmail);
        await existingSession.GetCurrentUserAsync();

        logger.LogInformation("Removing temporary session for user ({UserId}) {UserName}", session.UserId,
            session.UserNameOrEmail);
        await RemoveSessionAsync(session, false);

        return existingSession;
    }

    public async ValueTask RemoveSessionAsync(UserSessionService session, bool logout = true)
    {
        logger.LogInformation("Removing session for user ({UserId}) {UserName}", session.UserId,
            session.UserNameOrEmail);

        if (!_sessions.Contains(session))
        {
            logger.LogError("Tried to remove a session that does not exist for user ({UserId}) {UserName}",
                session.UserId, session.UserNameOrEmail);
            throw new InvalidOperationException("Session does not exist.");
        }

        _sessions.Remove(session);
        OnSessionRemoved(session);

        if (logout)
        {
            logger.LogInformation("Logging out session for user ({UserId}) {UserName}", session.UserId,
                session.UserNameOrEmail);
            try
            {
                await session.LogoutAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to logout session for user ({UserId}) {UserName}", session.UserId,
                    session.UserNameOrEmail);
            }
        }

        await session.DisposeAsync();

        await userSessionStorage.UpdateAsync(storage =>
        {
            if (session.UserId is { } userId)
                storage.Sessions.Remove(userId);
        });

        logger.LogInformation("Session removed for user ({UserId}) {UserName}", session.UserId,
            session.UserNameOrEmail);
    }

    private void OnSessionCreated(UserSessionService e)
    {
        SessionCreated?.Invoke(this, e);
    }

    private void OnSessionRemoved(UserSessionService e)
    {
        SessionRemoved?.Invoke(this, e);
    }
}