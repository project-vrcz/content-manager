using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Http;
using VRChatContentManager.ConnectCore.Models.Api.V1;

namespace VRChatContentManager.ConnectCore.Extensions;

internal static class HttpContextExtenstion
{
    public static async ValueTask<TValue?> ReadJsonWithErrorHandleAsync<TValue>(this HttpContext httpContext,
        JsonTypeInfo<TValue> typeInfo)
    {
        try
        {
            var jsonBody = await httpContext.Request.ReadFromJsonAsync(typeInfo);
            if (jsonBody is null)
            {
                await httpContext.Response.WriteProblemAsync(
                    ApiV1ProblemType.Undocumented,
                    StatusCodes.Status400BadRequest,
                    "Bad Request", "Request body is null or invalid.");
            }

            return jsonBody;
        }
        catch (Exception)
        {
            await httpContext.Response.WriteProblemAsync(
                ApiV1ProblemType.Undocumented,
                StatusCodes.Status400BadRequest,
                "Bad Request", "Request body is null or invalid.");
        }

        return default;
    }
}