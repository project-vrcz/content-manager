using System.Net;

namespace VRChatContentManager.Core;

public class ProgressStreamContent(
    Stream source,
    Action<long>? progressCallback = null,
    int bufferSize = ProgressStreamContent.DefaultBufferSize)
    : HttpContent
{
    private const int DefaultBufferSize = 81920; // 8KB
    private readonly Stream _source = source ?? throw new ArgumentNullException(nameof(source));

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        _source.Position = 0;
        
        var buffer = new byte[bufferSize];
        int read;
        while ((read = await _source.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
        {
            await stream.WriteAsync(buffer.AsMemory(0, read));
            progressCallback?.Invoke(read);
        }
    }

    protected override bool TryComputeLength(out long length)
    {
        if (_source.CanSeek)
        {
            length = _source.Length - _source.Position;
            return true;
        }
        
        length = -1;
        return false;
    }
}