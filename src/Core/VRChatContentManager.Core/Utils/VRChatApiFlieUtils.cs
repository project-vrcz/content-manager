using System.Buffers;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Blake2Fast;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Files;
using VRChatContentManager.Core.Services.VRChatApi;

namespace VRChatContentManager.Core.Utils;

public static partial class VRChatApiFlieUtils
{
    [GeneratedRegex(@"\/api\/1\/file\/(?<FileId>.+?)\/")]
    private static partial Regex GetFileIdFromAssetUrlRegex();

    public static string? TryGetFileIdFromAssetUrl(string assetUrl)
    {
        var regex = GetFileIdFromAssetUrlRegex();
        var match = regex.Match(assetUrl);

        if (!match.Success)
        {
            return null;
        }

        return match.Groups["FileId"].Value;
    }

    public static async ValueTask<string> GetMd5FromStreamForVRChatAsync(Stream stream,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var md5Hash = await MD5.HashDataAsync(stream, cancellationToken);
        stream.Position = 0;
        return Convert.ToBase64String(md5Hash);
    }

    public static async ValueTask<byte[]> GetSignatureFromStreamForVRChatAsync(Stream stream,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var hasher = Blake2b.CreateIncrementalHasher(32);
        var buffer = ArrayPool<byte>.Shared.Rent(4096);

        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
            hasher.Update(buffer.AsSpan(0, bytesRead));

        ArrayPool<byte>.Shared.Return(buffer);
        var hash = hasher.Finish();

        stream.Position = 0;
        return hash;
    }

    public static string GetMimeTypeFromExtension(string extension)
    {
        switch (extension)
        {
            case ".vrcw":
                return "application/x-world";
            case ".vrca":
                return "application/x-avatar";
            case ".vrcp":
                return "application/x-prop";
            case ".dll":
                return "application/x-msdownload";
            case ".unitypackage":
            case ".gz":
                return "application/gzip";
            case ".jpg":
                return "image/jpg";
            case ".png":
                return "image/png";
            case ".sig":
                return "application/x-rsync-signature";
            case ".delta":
                return "application/x-rsync-delta";
            default:
                return "application/octet-stream";
        }
    }
}