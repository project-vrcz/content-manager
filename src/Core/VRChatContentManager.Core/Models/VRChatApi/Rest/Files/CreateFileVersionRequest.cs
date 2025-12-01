using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Files;

public record CreateFileVersionRequest(
    [property: JsonPropertyName("fileMd5")]
    string FileMd5,
    [property: JsonPropertyName("fileSizeInBytes")]
    long FileSizeInBytes,
    [property: JsonPropertyName("signatureMd5")]
    string SignatureMd5,
    [property: JsonPropertyName("signatureSizeInBytes")]
    long SignatureSizeInBytes
);