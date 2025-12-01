using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using VRChatContentManager.ConnectCore.Models.Api.V1.Requests.Auth;
using VRChatContentManager.ConnectCore.Models.Api.V1.Requests.Task;
using VRChatContentManager.ConnectCore.Models.Api.V1.Responses.Auth;
using VRChatContentManager.ConnectCore.Models.Api.V1.Responses.Files;
using VRChatContentManager.ConnectCore.Models.Api.V1.Responses.Meta;

namespace VRChatContentManager.ConnectCore.Models.Api.V1;

[JsonSerializable(typeof(ApiV1MetadataResponse))]
[JsonSerializable(typeof(ApiV1AuthChallengeRequest))]
[JsonSerializable(typeof(ApiV1ChallengeResponse))]
[JsonSerializable(typeof(ApiV1RefreshTokenRequest))]
[JsonSerializable(typeof(ApiV1RequestChallengeRequest))]
[JsonSerializable(typeof(ApiV1AuthMetadataResponse))]
[JsonSerializable(typeof(ApiV1UploadFileResponse))]
[JsonSerializable(typeof(CreateWorldPublishTaskRequest))]
[JsonSerializable(typeof(CreateAvatarPublishTaskRequest))]
[JsonSerializable(typeof(ProblemDetails))]
public sealed partial class ApiV1JsonContext : JsonSerializerContext;