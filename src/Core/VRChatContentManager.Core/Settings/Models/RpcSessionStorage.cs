using VRChatContentManager.ConnectCore.Models.ClientSession;

namespace VRChatContentManager.Core.Settings.Models;

public sealed class RpcSessionStorage
{
    public List<RpcClientSession> Sessions { get; set; } = [];
    public string? SecretKey { get; set; }
    public string? IssuerKey { get; set; }
}