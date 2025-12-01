namespace VRChatContentManager.Core.Models.VRChatApi;

public class ApiErrorException(string message, int statusCode) : Exception(message)
{
    public override string Message => "Api request failed with status code " + StatusCode + ": " + base.Message;
    public string ApiErrorMessage => base.Message;

    public int StatusCode { get; init; } = statusCode;
}