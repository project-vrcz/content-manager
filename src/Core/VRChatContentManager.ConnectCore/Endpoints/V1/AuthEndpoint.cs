using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.ConnectCore.Extensions;
using VRChatContentManager.ConnectCore.Models.Api.V1;
using VRChatContentManager.ConnectCore.Models.Api.V1.Responses.Auth;
using VRChatContentManager.ConnectCore.Services.Connect;

namespace VRChatContentManager.ConnectCore.Endpoints.V1;

public static class AuthEndpoint
{
    public static EndpointService MapAuthEndpoints(this EndpointService endpoints)
    {
        endpoints.Map("POST", "/v1/auth/request-challenge", RequestChallenge);
        endpoints.Map("POST", "/v1/auth/challenge", Challenge);
        endpoints.Map("POST", "/v1/auth/refresh", RefreshToken);
        endpoints.Map("GET", "/v1/auth/metadata", GetMetadata);

        return endpoints;
    }

    private static async Task GetMetadata(HttpContext context, IServiceProvider _)
    {
        if (context.User.Identity is null) throw new InvalidOperationException("User identity is null.");

        var identity = context.User.Identity.Name;
        var expires = context.User.FindFirst("exp")?.Value;

        if (string.IsNullOrEmpty(identity) || string.IsNullOrEmpty(expires) ||
            !ulong.TryParse(expires, out var expUnix))
        {
            await context.Response.WriteProblemAsync(ApiV1ProblemType.Undocumented, StatusCodes.Status401Unauthorized,
                "Invalid Session", "The session is invalid or has expired. Please authenticate again.");
            return;
        }

        var response = new ApiV1AuthMetadataResponse(expUnix, identity);

        await context.Response.WriteAsJsonAsync(response, ApiV1JsonContext.Default.ApiV1AuthMetadataResponse);
    }

    private static async Task RefreshToken(HttpContext context, IServiceProvider services)
    {
        if (context.User.Identity?.Name is null)
            throw new InvalidOperationException("User identity is null.");

        if (await context.ReadJsonWithErrorHandleAsync(ApiV1JsonContext.Default.ApiV1RefreshTokenRequest) is not
            { } requestBody) return;

        var sessionService = services.GetRequiredService<ClientSessionService>();
        var sessionId = context.User.Identity.Name;
        var clientName = requestBody.ClientName;

        var jwt = await sessionService.RefreshSessionAsync(sessionId, clientName);
        await context.Response.WriteAsJsonAsync(new ApiV1ChallengeResponse(jwt),
            ApiV1JsonContext.Default.ApiV1ChallengeResponse);
    }

    private static async Task Challenge(HttpContext context, IServiceProvider services)
    {
        if (await context.ReadJsonWithErrorHandleAsync(ApiV1JsonContext.Default.ApiV1AuthChallengeRequest) is not
            { } requestBody) return;

        var sessionService = services.GetRequiredService<ClientSessionService>();

        string jwt;
        try
        {
            jwt = await sessionService.CreateSessionAsync(requestBody.Code, requestBody.ClientId,
                requestBody.IdentityPrompt);
        }
        catch (InvalidOperationException)
        {
            await context.Response.WriteProblemAsync(ApiV1ProblemType.Undocumented, StatusCodes.Status400BadRequest,
                "Code is invalid.", "The provided code is invalid or has expired.");
            return;
        }

        await context.Response.WriteAsJsonAsync(new ApiV1ChallengeResponse(jwt),
            ApiV1JsonContext.Default.ApiV1ChallengeResponse);
    }

    private static async Task RequestChallenge(HttpContext context, IServiceProvider services)
    {
        if (await context.ReadJsonWithErrorHandleAsync(ApiV1JsonContext.Default.ApiV1RequestChallengeRequest) is not
            { } requestBody) return;

        var sessionService = services.GetRequiredService<ClientSessionService>();
        var clientId = requestBody.ClientId;
        var identityPrompt = requestBody.IdentityPrompt;
        var clientName = requestBody.ClientName;

        await sessionService.CreateChallengeAsync(clientId, identityPrompt, clientName);

        context.Response.StatusCode = StatusCodes.Status204NoContent;
    }
}