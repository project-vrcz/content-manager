using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest.Files;

public record VRChatApiFile(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("versions")] VRChatApiFileVersion[] Versions
);

public record VRChatApiFileVersion(
    [property: JsonPropertyName("version")]
    int Version,
    [property: JsonPropertyName("status")]
    string Status,
    [property: JsonPropertyName("file")]
    VRChatApiFileVersionEntity? File,
    [property: JsonPropertyName("signature")]
    VRChatApiFileVersionEntity? Signature
);

public record VRChatApiFileVersionEntity(
    [property: JsonPropertyName("sizeInBytes")] long SizeInBytes,
    [property: JsonPropertyName("md5")] string Md5,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("category")] string Category
);

public enum VRChatApiFileType
{
    File,
    Signature
}

public static class VRChatApiFileTypeExtensions
{
    public static string ToApiString(this VRChatApiFileType fileType) => fileType switch
    {
        VRChatApiFileType.File => "file",
        VRChatApiFileType.Signature => "signature",
        _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
    };
}