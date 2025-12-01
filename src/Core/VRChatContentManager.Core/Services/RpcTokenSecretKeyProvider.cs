using System.Security.Cryptography;
using VRChatContentManager.ConnectCore.Services.Connect.SessionStorage;
using VRChatContentManager.Core.Settings;
using VRChatContentManager.Core.Settings.Models;

namespace VRChatContentManager.Core.Services;

public sealed class RpcTokenSecretKeyProvider(IWritableOptions<RpcSessionStorage> storage) : ITokenSecretKeyProvider
{
    public async ValueTask<byte[]> GetSecretKeyAsync()
    {
        if (storage.Value.SecretKey is { } secretKey)
        {
            var secretKeyBytes = new byte[64];
            if (Convert.TryFromBase64String(secretKey, secretKeyBytes, out var bytesWritten) && bytesWritten == 64)
            {
                return secretKeyBytes;
            }
        }
        
        var newKey = GenerateKey();
        await storage.UpdateAsync(s => s.SecretKey = Convert.ToBase64String(newKey));
        return newKey;
    }

    private static byte[] GenerateKey()
    {
        using var sha256 = new HMACSHA256();
        return sha256.Key;
    }
}