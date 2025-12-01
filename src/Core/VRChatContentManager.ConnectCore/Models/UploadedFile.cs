namespace VRChatContentManager.ConnectCore.Models;

public sealed class UploadedFile
{
    public required Stream FileStream { get; set; }
    public required string FileName { get; set; }
}