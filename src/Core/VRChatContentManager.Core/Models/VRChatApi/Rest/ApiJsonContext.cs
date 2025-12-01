using System.Text.Json.Serialization;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Auth;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Avatars;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Files;
using VRChatContentManager.Core.Models.VRChatApi.Rest.UnityPackages;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Worlds;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest;

// Response
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(RequireTwoFactorAuthResponse))]
[JsonSerializable(typeof(FileUploadUrlResponse))]

// Request
[JsonSerializable(typeof(CreateWorldRequest))]
[JsonSerializable(typeof(CreateAvatarVersionRequest))]
[JsonSerializable(typeof(CreateWorldVersionRequest))]
[JsonSerializable(typeof(CreateFileRequest))]
[JsonSerializable(typeof(CreateFileVersionRequest))]
[JsonSerializable(typeof(CompleteFileUploadRequest))]
[JsonSerializable(typeof(VerifyTotpRequest))]

// Entity
[JsonSerializable(typeof(VRChatApiAvatar))]
[JsonSerializable(typeof(VRChatApiAvatar[]))]
[JsonSerializable(typeof(VRChatApiWorld))]
[JsonSerializable(typeof(VRChatApiUnityPackage))]
[JsonSerializable(typeof(CurrentUser))]
[JsonSerializable(typeof(VRChatApiFile))]
[JsonSerializable(typeof(VRChatApiFileVersion))]
[JsonSerializable(typeof(FileVersionUploadStatus))]
[JsonSerializable(typeof(Requires2FA))]
public sealed partial class ApiJsonContext : JsonSerializerContext;