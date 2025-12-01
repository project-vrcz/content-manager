using Avalonia.Media.Imaging;
using Microsoft.Extensions.Caching.Memory;
using VRChatContentManager.Core;
using VRChatContentManager.Core.Services.App;

namespace VRChatContentManager.App.Shared;

public sealed class AppWebImageLoader
{
    private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;

    private readonly RemoteImageService _remoteImageService;
    private readonly IMemoryCache _memoryCache;

    private const string CacheKeyPrefix = "VRCZ.App.AppWebImageLoader.Cache-";

    public static AppWebImageLoader Instance
    {
        get
        {
            if (field is not null)
                return field;

            var httpClient = new HttpClient();
            httpClient.AddUserAgent();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            field = new AppWebImageLoader(new RemoteImageService(httpClient, memoryCache), memoryCache);
            return field;
        }
        set;
    }

    public AppWebImageLoader(RemoteImageService remoteImageService, IMemoryCache memoryCache)
    {
        _remoteImageService = remoteImageService;
        _memoryCache = memoryCache;

        _memoryCacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(30)
        };

        _memoryCacheEntryOptions.RegisterPostEvictionCallback((_, cacheEntry, _, _) =>
        {
            (cacheEntry as Bitmap)?.Dispose();

            GC.Collect();
        });
    }

    public async Task<Bitmap?> ProvideImageAsync(string url)
    {
        return await ProvideImageAsyncWithSize(url);
    }

    public async Task<Bitmap?> ProvideImageAsyncWithSize(string url, int? width = null, int? height = null)
    {
        var decodeSize = GetDecodeSize(width, height);
        var cacheKey =
            $"{CacheKeyPrefix}{(decodeSize is null ? "raw" : $"{decodeSize.Value.IsHeight}-{decodeSize.Value.Size}")}{url}";

        if (_memoryCache.TryGetValue<Bitmap>(cacheKey, out var cachedBitmap))
            return cachedBitmap;

        var bitmap = await ProvideImageAsyncWithSizeCore(url, decodeSize);

        _memoryCache.Set(cacheKey, bitmap, _memoryCacheEntryOptions);

        return bitmap;
    }

    private async ValueTask<Bitmap?> ProvideImageAsyncWithSizeCore(string url, DecodeSize? decodeSize)
    {
        var memoryStream = await _remoteImageService.GetImageStreamAsync(url);

        if (!decodeSize.HasValue)
            return await Task.Run(() => new Bitmap(memoryStream));

        if (decodeSize.Value.IsHeight)
            return await Task.Run(() => Bitmap.DecodeToHeight(memoryStream, decodeSize.Value.Size));

        return await Task.Run(() => Bitmap.DecodeToWidth(memoryStream, decodeSize.Value.Size));
    }

    private struct DecodeSize(int size, bool isHeight)
    {
        public int Size { get; } = size;
        public bool IsHeight { get; } = isHeight;
    }

    private DecodeSize? GetDecodeSize(int? width, int? height)
    {
        if (height is not null && width is not null)
        {
            if (height >= width)
                return new DecodeSize((int)height, true);

            return new DecodeSize((int)width, false);
        }

        if (width is not null && height is null)
        {
            return new DecodeSize((int)width, false);
        }

        if (height is not null && width is null)
        {
            return new DecodeSize((int)height, true);
        }

        return null;
    }
}