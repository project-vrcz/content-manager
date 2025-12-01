using VRChatContentManager.ConnectCore.Models;

namespace VRChatContentManager.ConnectCore.Services;

public interface IFileService
{
    ValueTask<UploadFileTask> GetUploadFileStreamAsync(string fileName);

    ValueTask<Stream?> GetFileAsync(string fileId);
    
    ValueTask<UploadedFile?> GetFileWithNameAsync(string fileId);
    
    ValueTask DeleteFileAsync(string fileId);
}