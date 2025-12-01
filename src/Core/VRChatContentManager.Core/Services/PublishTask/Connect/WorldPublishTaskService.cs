using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VRChatContentManager.ConnectCore.Services.PublishTask;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Worlds;
using VRChatContentManager.Core.Services.PublishTask.ContentPublisher;
using VRChatContentManager.Core.Services.UserSession;

namespace VRChatContentManager.Core.Services.PublishTask.Connect;

public class WorldPublishTaskService(
    UserSessionManagerService userSessionManagerService,
    WorldContentPublisherFactory contentPublisherFactory,
    ILogger<WorldPublishTaskService> logger) : IWorldPublishTaskService
{
    public async ValueTask<string> CreatePublishTaskAsync(
        string worldId,
        string worldBundleFileId,
        string worldName,
        string platform,
        string unityVersion,
        string? authorId,
        string? worldSignature,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        int? capacity,
        int? recommendedCapacity,
        string? previewYoutubeId,
        string[]? udonProducts)
    {
        var userSession = await GetUserSessionByWorldIdAsync(worldId, authorId);

        var scope = await userSession.CreateOrGetSessionScopeAsync();

        var taskManager = scope.ServiceProvider.GetRequiredService<TaskManagerService>();
        var contentPublisher =
            contentPublisherFactory.Create(
                userSession,
                worldId,
                worldName,
                platform,
                unityVersion,
                worldSignature,
                capacity,
                recommendedCapacity,
                previewYoutubeId,
                udonProducts
            );

        var task = await taskManager.CreateTask(
            worldId,
            worldBundleFileId,
            thumbnailFileId,
            description,
            tags,
            releaseStatus,
            contentPublisher
        );
        task.Start();

        return task.TaskId;
    }

    private async ValueTask<UserSessionService> GetUserSessionByWorldIdAsync(string worldId,
        string? requestUserId = null)
    {
        if (requestUserId is not null)
        {
            if (userSessionManagerService.Sessions
                    .FirstOrDefault(session => session.UserId == requestUserId) is not { } requestSession)
                throw new ArgumentException("The specified user session was not found.", nameof(requestUserId));

            VRChatApiWorld requestUserWorld;
            try
            {
                requestUserWorld = await requestSession.GetApiClient().GetWorldAsync(worldId);
            }
            catch (ApiErrorException ex) when (ex.StatusCode == 404)
            {
                logger.LogInformation(ex, "The world {WorldId} was not found for user {UserId}. Will create new world.",
                    worldId, requestUserId);
                return requestSession;
            }

            if (requestUserWorld.AuthorId != requestUserId)
                throw new ArgumentException("The specified user does not own the world.", nameof(requestUserId));

            return requestSession;
        }

        if (userSessionManagerService.Sessions.Count == 0)
            throw new InvalidOperationException("No user sessions are available.");

        var initialSession = userSessionManagerService.Sessions[0];

        VRChatApiWorld world;
        try
        {
            world = await initialSession.GetApiClient().GetWorldAsync(worldId);
        }
        catch (ApiErrorException ex) when (ex.StatusCode == 404)
        {
            throw new ArgumentException("The specified world was not found.", nameof(worldId), ex);
        }

        var ownerSession = userSessionManagerService.Sessions
            .FirstOrDefault(session => session.UserId == world.AuthorId);

        if (ownerSession is null)
            throw new InvalidOperationException("The owner of the world does not have an active user session");

        return ownerSession;
    }
}