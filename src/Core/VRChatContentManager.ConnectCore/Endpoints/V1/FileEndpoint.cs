using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using VRChatContentManager.ConnectCore.Extensions;
using VRChatContentManager.ConnectCore.Models;
using VRChatContentManager.ConnectCore.Models.Api.V1;
using VRChatContentManager.ConnectCore.Models.Api.V1.Responses.Files;
using VRChatContentManager.ConnectCore.Services;
using VRChatContentManager.ConnectCore.Services.Connect;

namespace VRChatContentManager.ConnectCore.Endpoints.V1;

public static class FileEndpoint
{
    public static EndpointService MapFileEndpoints(this EndpointService endpoints)
    {
        endpoints.Map("POST", "/v1/files", UploadFile);
        return endpoints;
    }

    private static async Task UploadFile(HttpContext context, IServiceProvider services)
    {
        var fileService = services.GetRequiredService<IFileService>();

        var boundary = context.Request.GetMultipartBoundary();
        if (string.IsNullOrEmpty(boundary))
        {
            await context.Response.WriteProblemAsync(ApiV1ProblemType.Undocumented, StatusCodes.Status400BadRequest,
                "Invalid Request", "Not a multipart request.");
            return;
        }


        UploadFileTask? uploadFileTask = null;
        var reader = new MultipartReader(boundary, context.Request.Body);

        var fileRead = false;
        var badRequest = false;
        while (await reader.ReadNextSectionAsync() is { } section)
        {
            var contentDisposition = section.GetContentDispositionHeader();

            if (contentDisposition != null &&
                contentDisposition.Name == "file" &&
                contentDisposition.IsFileDisposition())
            {
                if (fileRead)
                {
                    badRequest = true;
                    continue;
                }

                fileRead = true;
                uploadFileTask = await fileService.GetUploadFileStreamAsync(contentDisposition.FileName.ToString());
                await using var stream = uploadFileTask.FileStream;
                await section.Body.CopyToAsync(stream);
            }
        }

        if (!fileRead || uploadFileTask is null)
        {
            await context.Response.WriteProblemAsync(ApiV1ProblemType.Undocumented, StatusCodes.Status400BadRequest,
                "Invalid Request", "Request does not contain a file disposition named \"file\".");
            return;
        }

        if (badRequest)
        {
            await context.Response.WriteProblemAsync(ApiV1ProblemType.Undocumented, StatusCodes.Status400BadRequest,
                "Invalid Request", "Request contains more than one file disposition named \"file\".");
            return;
        }

        await context.Response.WriteAsJsonAsync(new ApiV1UploadFileResponse(uploadFileTask.FileId),
            ApiV1JsonContext.Default.ApiV1UploadFileResponse);
    }
}