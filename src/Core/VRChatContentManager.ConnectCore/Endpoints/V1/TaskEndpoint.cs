using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.ConnectCore.Extensions;
using VRChatContentManager.ConnectCore.Models.Api.V1;
using VRChatContentManager.ConnectCore.Services.Connect;
using VRChatContentManager.ConnectCore.Services.PublishTask;

namespace VRChatContentManager.ConnectCore.Endpoints.V1;

public static class TaskEndpoint
{
    public static EndpointService MapTaskEndpoint(this EndpointService service)
    {
        service.Map("POST", "/v1/tasks/world", CreateWorldPublishTask);
        service.Map("POST", "/v1/tasks/avatar", CreateAvatarPublishTask);

        return service;
    }

    private static async Task CreateWorldPublishTask(HttpContext context, IServiceProvider services)
    {
        if (await context.ReadJsonWithErrorHandleAsync(ApiV1JsonContext.Default.CreateWorldPublishTaskRequest) is not
            { } request)
            return;

        var worldPublishTaskService = services.GetRequiredService<IWorldPublishTaskService>();
        await worldPublishTaskService.CreatePublishTaskAsync(
            request.WorldId,
            request.WorldBundleFileId,
            request.Name,
            request.Platform,
            request.UnityVersion,
            request.AuthorId,
            request.WorldSignature,
            request.ThumbnailFileId,
            request.Description,
            request.Tags,
            request.ReleaseStatus,
            request.Capacity,
            request.RecommendedCapacity,
            request.PreviewYoutubeId,
            request.UdonProducts
        );
    }

    private static async Task CreateAvatarPublishTask(HttpContext context, IServiceProvider services)
    {
        if (await context.ReadJsonWithErrorHandleAsync(ApiV1JsonContext.Default.CreateAvatarPublishTaskRequest) is not
            { } request)
            return;

        var avatarPublishTaskService = services.GetRequiredService<IAvatarPublishTaskService>();
        await avatarPublishTaskService.CreatePublishTaskAsync(
            request.AvatarId,
            request.AvatarBundleFileId,
            request.Name,
            request.Platform,
            request.UnityVersion,
            request.ThumbnailFileId,
            request.Description,
            request.Tags,
            request.ReleaseStatus);
    }
}