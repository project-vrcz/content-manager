using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VRChatContentManager.ConnectCore.Models.ClientSession;
using VRChatContentManager.ConnectCore.Services.Connect.Challenge;
using VRChatContentManager.ConnectCore.Services.Connect.SessionStorage;

namespace VRChatContentManager.ConnectCore.Services.Connect;

public sealed class ClientSessionService(
    ILogger<ClientSessionService> logger,
    IRequestChallengeService requestChallengeService,
    ISessionStorageService sessionStorageService,
    ITokenSecretKeyProvider tokenSecretKeyProvider)
{
    private const string IssuerPrefix = "vrchat-content-manager";
    private const string Subject = "content-manager-build-pipeline-rpc";

    private readonly TimeSpan _expiry = TimeSpan.FromDays(14);

    private readonly Lock _challengeSessionLock = new();
    private readonly List<ChallengeSession> _challengeSessions = [];

    public async ValueTask<TokenValidationResult> ValidateJwtAsync(string jwt)
    {
        var securityKey = await tokenSecretKeyProvider.GetSecretKeyAsync();
        var issuer = await GetIssuerAsync();

        var tokenHandler = new JwtSecurityTokenHandler();
        var result = await tokenHandler.ValidateTokenAsync(jwt, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(securityKey),
            ValidIssuer = issuer,
            ValidAlgorithms =
            [
                SecurityAlgorithms.HmacSha256
            ],
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireAudience = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            NameClaimType = JwtRegisteredClaimNames.Aud,
            AudienceValidator = (audiences, _, _) =>
            {
                foreach (var audience in audiences)
                {
                    if (sessionStorageService.GetSessionByClientId(audience) is { } session &&
                        session.Expires > DateTimeOffset.UtcNow)
                        return true;
                }

                return false;
            }
        });

        return result;
    }

    public async ValueTask<string> CreateChallengeAsync(string clientId, string identityPrompt, string clientName)
    {
        await CleanupExpiredSessionsAsync();

        logger.LogInformation("Creating challenge for client {ClientId}", clientId);
        await sessionStorageService.RemoveSessionByClientIdAsync(clientId);

        var code = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();

        lock (_challengeSessionLock)
        {
            if (_challengeSessions.FirstOrDefault(challenge => challenge.ClientId == clientId) is { } session)
                _challengeSessions.Remove(session);

            var expires = DateTimeOffset.UtcNow.AddMinutes(5);

            var challengeSession = new ChallengeSession(code, clientId, identityPrompt, clientName, expires);
            _challengeSessions.Add(challengeSession);
        }

        await requestChallengeService.RequestChallengeAsync(code, clientId, identityPrompt, clientName);

        return code;
    }

    public async ValueTask<string> CreateSessionAsync(string code, string clientId, string identityPrompt)
    {
        await CleanupExpiredSessionsAsync();
        await sessionStorageService.RemoveSessionByClientIdAsync(clientId);

        logger.LogInformation("Creating session for client {ClientId}", clientId);
        if (TryGetChallengeSession(code, clientId, identityPrompt) is not { } challengeSession)
            throw new InvalidOperationException("Challenge session not found");

        await requestChallengeService.CompleteChallengeAsync(clientId);

        var clientName = challengeSession.ClientName;
        var expires = DateTimeOffset.UtcNow + _expiry;

        var session = new RpcClientSession(clientId, expires, clientName);
        await sessionStorageService.AddSessionAsync(session);

        return await GenerateJwtAsync(clientId);
    }

    public async ValueTask<string> RefreshSessionAsync(string clientId, string? clientName)
    {
        await CleanupExpiredSessionsAsync();

        logger.LogInformation("Refreshing session for client {ClientId}", clientId);
        if (sessionStorageService.GetSessionByClientId(clientId) is not { } existingSession)
        {
            throw new InvalidOperationException("No existing session found for the given client ID.");
        }

        await sessionStorageService.RemoveSessionByClientIdAsync(clientId);

        var expires = DateTimeOffset.UtcNow + _expiry;
        var newSession = new RpcClientSession(clientId, expires, clientName ?? existingSession.ClientName);
        await sessionStorageService.AddSessionAsync(newSession);

        return await GenerateJwtAsync(clientId);
    }

    private ChallengeSession? TryGetChallengeSession(string code, string clientId, string identityPrompt)
    {
        lock (_challengeSessionLock)
        {
            return _challengeSessions.FirstOrDefault(session =>
                session.Code == code &&
                session.ClientId == clientId &&
                session.IdentityPrompt == identityPrompt);
        }
    }

    private async Task CleanupExpiredSessionsAsync()
    {
        var now = DateTimeOffset.UtcNow;

        await sessionStorageService.RemoveExpiredSessionsAsync();
        lock (_challengeSessionLock)
        {
            _challengeSessions.RemoveAll(session => session.Expires <= now);
        }
    }

    private async Task<string> GenerateJwtAsync(string clientId)
    {
        var currentDateTime = DateTimeOffset.UtcNow;

        var issuer = await GetIssuerAsync();

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Iss, issuer),
            new(JwtRegisteredClaimNames.Sub, Subject),
            new(JwtRegisteredClaimNames.Aud, clientId),
            new(JwtRegisteredClaimNames.Exp, (currentDateTime + _expiry).ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Iat, currentDateTime.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Nbf, currentDateTime.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        ];

        var securityKey = await tokenSecretKeyProvider.GetSecretKeyAsync();
        var key = new SymmetricSecurityKey(securityKey);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async ValueTask<string> GetIssuerAsync()
    {
        var issuerKey = await sessionStorageService.GetIssuerAsync();
        return $"{IssuerPrefix}-{issuerKey}";
    }
}