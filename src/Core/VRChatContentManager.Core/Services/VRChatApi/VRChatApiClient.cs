using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Models.VRChatApi.Rest;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Auth;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Avatars;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Files;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Worlds;
using VRChatContentManager.Core.Services.VRChatApi.S3;

namespace VRChatContentManager.Core.Services.VRChatApi;

public sealed partial class VRChatApiClient(
    HttpClient httpClient,
    ILogger<VRChatApiClient> logger,
    ConcurrentMultipartUploaderFactory concurrentMultipartUploaderFactory)
{
    public async ValueTask<CurrentUser> GetCurrentUser()
    {
        var response = await httpClient.GetAsync("auth/user");

        await HandleErrorResponseAsync(response);

        var user = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.CurrentUser);
        if (user is null)
            throw new UnexpectedApiBehaviourException("The API response a null user object.");

        return user;
    }

    public async ValueTask<LoginResult> LoginAsync(string username, string password)
    {
        var token = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{Uri.EscapeDataString(username)}:{Uri.EscapeDataString(password)}"));

        var request = new HttpRequestMessage(HttpMethod.Get, "auth/user")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Basic", token)
            }
        };

        var response = await httpClient.SendAsync(request);

        await HandleErrorResponseAsync(response);

        var content = await response.Content.ReadAsStringAsync();
        var responseJson = JsonNode.Parse(content);

        if (responseJson is null)
            throw new UnexpectedApiBehaviourException("The API returned a null json response.");

        if (responseJson["requiresTwoFactorAuth"] is { } twoFactorAuthField)
        {
            if (twoFactorAuthField.GetValueKind() != JsonValueKind.Array)
                throw new UnexpectedApiBehaviourException(
                    "The API returned a json response with not array requiresTwoFactorAuth field.");

            var requires2FaResponse =
                responseJson.Deserialize(ApiJsonContext.Default.RequireTwoFactorAuthResponse);
            return new LoginResult(false, requires2FaResponse!.RequiresTwoFactorAuth);
        }

        return new LoginResult(true, []);
    }

    public async ValueTask<bool> VerifyOtpAsync(string code, bool isEmailOtp = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Post,
            isEmailOtp ? "auth/twofactorauth/emailotp/verify" : "auth/twofactorauth/totp/verify")
        {
            Content = JsonContent.Create(new VerifyTotpRequest(code), ApiJsonContext.Default.VerifyTotpRequest)
        };

        var response = await httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
        {
            var responseJson = JsonNode.Parse(content);
            if (responseJson is null)
                throw new UnexpectedApiBehaviourException("The API returned a null json response.");

            if (responseJson["verified"] is { } verifiedField)
            {
                if (verifiedField.GetValueKind() != JsonValueKind.False &&
                    verifiedField.GetValueKind() != JsonValueKind.True)
                    throw new UnexpectedApiBehaviourException(
                        "The API returned a json response with not boolean verified field.");

                return verifiedField.GetValue<bool>();
            }
        }

        if (!response.IsSuccessStatusCode)
            HandleErrorResponse(content);

        throw new UnexpectedApiBehaviourException(
            $"The API returned a json response without verified field which status code {response.StatusCode}.");
    }

    public async Task LogoutAsync()
    {
        var response = await httpClient.PutAsync("logout", null);
        await HandleErrorResponseAsync(response);
    }

    private static async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var content = await response.Content.ReadAsStringAsync();
        HandleErrorResponse(content);
    }

    private static void HandleErrorResponse(string response)
    {
        var errorResponse = JsonSerializer.Deserialize(response, ApiJsonContext.Default.ApiErrorResponse);

        if (errorResponse is null)
            throw new UnexpectedApiBehaviourException(
                "The API returned an error response that could not be deserialized.");

        throw new ApiErrorException(errorResponse.Message, errorResponse.StatusCode);
    }
}