using VRChatContentManager.ConnectCore.Models;
using VRChatContentManager.ConnectCore.Services;
using VRChatContentManager.Core.Services.App;

namespace VRChatContentManager.Core.Services;

public sealed class TempFileService : IFileService
{
    private readonly Dictionary<string, FileMapEntry> _fileMap = [];
    
    public ValueTask<UploadFileTask> GetUploadFileStreamAsync(string fileName)
    {
        var rootPath = GetTempFileRootPath();

        var fileId = Guid.NewGuid().ToString("N");
        var filePath = Path.Combine(rootPath, fileId);

        var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.Asynchronous);
        var uploadFileTask = new UploadFileTask(fileStream, fileId);
        
        _fileMap[fileId] = new FileMapEntry(fileName, filePath);

        return ValueTask.FromResult(uploadFileTask);
    }

    public ValueTask<Stream?> GetFileAsync(string fileId)
    {
        if (!_fileMap.TryGetValue(fileId, out var fileMapEntry))
            return ValueTask.FromResult<Stream?>(null);
        
        return ValueTask.FromResult<Stream?>(File.OpenRead(fileMapEntry.FilePath));
    }

    public ValueTask<UploadedFile?> GetFileWithNameAsync(string fileId)
    {
        if (!_fileMap.TryGetValue(fileId, out var fileMapEntry))
            return ValueTask.FromResult<UploadedFile?>(null);
        
        var fileName = Path.GetFileName(fileMapEntry.FileNmae);
        var fileStream = File.OpenRead(fileMapEntry.FilePath);
        
        var uploadedFile = new UploadedFile
        {
            FileName = fileName,
            FileStream = fileStream
        };
        
        return ValueTask.FromResult<UploadedFile?>(uploadedFile);
    }

    public ValueTask DeleteFileAsync(string fileId)
    {
        if (_fileMap.TryGetValue(fileId, out var fileMapEntry))
        {
            if (File.Exists(fileMapEntry.FilePath))
            {
                File.Delete(fileMapEntry.FilePath);
            }
            
            _fileMap.Remove(fileId);
        }

        return ValueTask.CompletedTask;
    }

    private string GetTempFileRootPath()
    {
        var tempPath = AppStorageService.GetTempPath();
        var rootPath = Path.Combine(tempPath, "rpc-temp-files");

        if (!Directory.Exists(rootPath))
            Directory.CreateDirectory(rootPath);

        return rootPath;
    }
    
    private record FileMapEntry(string FileNmae, string FilePath);
}