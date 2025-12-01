using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;
using VRChatContentManager.ConnectCore.Services;

namespace VRChatContentManager.Core.Services.PublishTask.BundleProcesser;

public sealed class BundleCompressProcesser(
    IFileService fileService,
    ILogger<BundleCompressProcesser> logger)
    : IBundleProcesser
{
    public async ValueTask<string> ProcessBundleAsync(
        string bundleFileId,
        PublishStageProgressReporter? progressReporter = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        progressReporter?.Report("Preparing to compress bundle...");

        await using var rawBundleFileStream = await fileService.GetFileAsync(bundleFileId);
        if (rawBundleFileStream is null)
            throw new InvalidOperationException("Bundle file with provided id is not found.");

        var createOutputBundleFileTask =
            await fileService.GetUploadFileStreamAsync(Guid.NewGuid() + "-" + bundleFileId);
        var outputBundleFileStream = createOutputBundleFileTask.FileStream;

        try
        {
            await Task.Factory.StartNew(() =>
            {
                logger.LogInformation("Reading bundle file {BundleFileId}", bundleFileId);
                progressReporter?.Report("Reading bundle file...");

                var bundleFile = new AssetBundleFile();

                using var bundleReader = new AssetsFileReader(rawBundleFileStream);
                bundleFile.Read(bundleReader);

                if (bundleFile.DataIsCompressed)
                {
                    logger.LogInformation("Decompressing bundle file {BundleFileId}", bundleFileId);
                    progressReporter?.Report("Decompressing bundle file for recompression...");

                    var newBundleFile = BundleHelper.UnpackBundle(bundleFile, cancellationToken: cancellationToken);
                    bundleFile.Close();
                    bundleFile = newBundleFile;
                }

                cancellationToken.ThrowIfCancellationRequested();

                logger.LogInformation("Compressing bundle file {BundleFileId}", bundleFileId);
                progressReporter?.Report("Compressing bundle file...");

                using var writer = new AssetsFileWriter(outputBundleFileStream);
                bundleFile.Pack(writer, AssetBundleCompressionType.LZMA, cancellationToken: cancellationToken);
                bundleFile.Close();
            }, TaskCreationOptions.LongRunning);

            return createOutputBundleFileTask.FileId;
        }
        catch
        {
            await fileService.DeleteFileAsync(createOutputBundleFileTask.FileId);
            throw;
        }
    }
}