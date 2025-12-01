using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Caching.Memory;

namespace VRChatContentManager.Core.Services.App;

public sealed class RemoteImageService(HttpClient httpClient, IMemoryCache memoryCache)
{
    private const string MemoryCacheKeyPrefix = "remote-image-";

    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<Stream> GetImageStreamAsync(string url)
    {
        var uri = new Uri(url);

        if (GetMemoryCache(uri) is { } memoryCacheStream)
            return memoryCacheStream;

        if (await GetDiskCache(url) is { } diskCacheCache)
            return diskCacheCache;

        var bytes = await httpClient.GetByteArrayAsync(url);

        SetMemoryCache(uri, bytes);
        await SetDiskCache(url, bytes);

        return new MemoryStream(bytes);
    }

    private void SetMemoryCache(Uri url, byte[] bytes)
    {
        var key = GetMemoryCacheKey(url);

        if (memoryCache.TryGetValue(key, out _))
            return;

        memoryCache.Set(key, bytes, new MemoryCacheEntryOptions
        {
            SlidingExpiration = _cacheDuration
        });
    }

    private MemoryStream? GetMemoryCache(Uri url)
    {
        var key = GetMemoryCacheKey(url);

        if (memoryCache.Get<byte[]>(key) is { } bytes)
            return new MemoryStream(bytes);

        return null;
    }

    internal string GetMemoryCacheKey(Uri url)
    {
        if (IsVRChatFiles(url))
            return MemoryCacheKeyPrefix + GetVRChatFileCacheKey(url.AbsolutePath);

        return MemoryCacheKeyPrefix + url;
    }

    private async Task SetDiskCache(string url, byte[] bytes)
    {
        var cacheFilePath = GetDiskCacheFilePath(url);

        var directoryName = Path.GetDirectoryName(cacheFilePath)!;

        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);

        await File.WriteAllBytesAsync(cacheFilePath, bytes);
    }

    private async Task<MemoryStream?> GetDiskCache(string url)
    {
        var cacheFilePath = GetDiskCacheFilePath(url);

        if (!File.Exists(cacheFilePath))
            return null;

        await using var cacheFileStream = File.OpenRead(cacheFilePath);
        var memoryStream = new MemoryStream();

        await cacheFileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    private string GetDiskCacheFilePath(string url)
    {
        var uri = new Uri(url);

        if (!IsVRChatFiles(uri))
        {
            // Normalize the URL then hash it
            var uriHash = GetMD5(uri.ToString());
            return Path.Join("common", uriHash);
        }

        var cacheKey = GetVRChatFileCacheKey(uri.AbsolutePath);

        return Path.Combine(AppStorageService.GetTempPath(), "images", cacheKey);
    }

    internal string GetVRChatFileCacheKey(string urlPath)
    {
        var pathParts = urlPath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        pathParts = pathParts[3..]
            .Select(part =>
            {
                // Prevent Invalid Path Characters
                var urlEncodedPart = HttpUtility.UrlEncode(part);

                // For case insensitivity file systems
                var hash = GetShortMD5(part);

                return urlEncodedPart + "_" + hash;
            })
            .ToList();

        // 8c7dd is short MD5 of "file"
        if (pathParts.Count == 2)
            pathParts.Add("file_8c7dd");

        pathParts.Insert(0, "vrchat-files");

        return Path.Join(pathParts.ToArray());
    }

    internal bool IsVRChatFiles(Uri uri)
    {
        return uri.Host is "vrchat.com" or "api.vrchat.cloud" &&
               (uri.AbsolutePath.StartsWith("/api/1/image/") || uri.AbsolutePath.StartsWith("/api/1/file/"));
    }

    internal static string GetShortMD5(string input)
    {
        return GetMD5(input).Substring(0, 5);
    }

    internal static string GetMD5(string input)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexStringLower(hash);
    }
}