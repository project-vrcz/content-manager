namespace VRChatContentManager.ConnectCore.Models;

public class UploadFileTask(Stream fileStream, string fileId)
{
    public Stream FileStream { get; } = fileStream;
    public string FileId { get; } = fileId;
}