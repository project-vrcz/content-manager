using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRChatContentManager.Core.Models.VRChatApi.Rest;

[JsonConverter(typeof(ApiErrorResponseConverter))]
public record ApiErrorResponse(
    string Message,
    int StatusCode
);

public class ApiErrorResponseConverter : JsonConverter<ApiErrorResponse>
{
    public override ApiErrorResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Failed to parse ApiErrorResponse: Expected StartObject token.");
        }

        string? message = null;
        int? statusCode = null;

        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.GetString();
            
            if (propertyName == "error")
                break;
        }

        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;
            
            var propertyName = reader.GetString()!;
            reader.Read();

            switch (propertyName)
            {
                case "message":
                    message = reader.GetString();
                    break;
                case "status_code":
                    statusCode = reader.GetInt32();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (message == null || statusCode == null)
        {
            throw new JsonException("Missing required properties");
        }

        return new ApiErrorResponse(message, statusCode.Value);
    }

    public override void Write(Utf8JsonWriter writer, ApiErrorResponse value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("message", value.Message);
        writer.WriteNumber("statusCode", value.StatusCode);
        writer.WriteEndObject();
    }
}