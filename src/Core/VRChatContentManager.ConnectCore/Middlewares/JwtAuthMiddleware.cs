using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VRChatContentManager.ConnectCore.Extensions;
using VRChatContentManager.ConnectCore.Models.Api.V1;
using VRChatContentManager.ConnectCore.Services;
using VRChatContentManager.ConnectCore.Services.Connect;

namespace VRChatContentManager.ConnectCore.Middlewares;

public class JwtAuthMiddleware(ClientSessionService clientSessionService) : MiddlewareBase
{
    public override async Task ExecuteAsync(HttpContext context, Func<Task> next)
    {
        if (context.Request.Path.StartsWithSegments("/v1/auth/challenge") ||
            context.Request.Path.StartsWithSegments("/v1/auth/request-challenge") ||
            context.Request.Path.StartsWithSegments("/v1/meta"))
        {
            await next();
            return;
        }

        var jwtHeader = context.Request.Headers.Authorization.ToString();
        if (!jwtHeader.StartsWith("Bearer "))
        {
            await WriteUnauthorizedAsync(context.Response);
            return;
        }

        var jwt = jwtHeader["Bearer ".Length..].Trim();
        var validateResult = await clientSessionService.ValidateJwtAsync(jwt);
        if (!validateResult.IsValid)
        {
            await WriteUnauthorizedAsync(context.Response);
            return;
        }
        
        context.User = new ClaimsPrincipal(validateResult.ClaimsIdentity);
        await next();
    }

    private async Task WriteUnauthorizedAsync(HttpResponse response)
    {
        await response.WriteProblemAsync(ApiV1ProblemType.Undocumented, StatusCodes.Status401Unauthorized,
            "Invalid Session",
            "The session is invalid or has expired. Please authenticate again.");
    }
}