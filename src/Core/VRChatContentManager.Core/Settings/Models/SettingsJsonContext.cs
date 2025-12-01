using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Settings.Models;

[JsonSerializable(typeof(UserSessionStorage))]
[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(RpcSessionStorage))]
public partial class SettingsJsonContext : JsonSerializerContext;