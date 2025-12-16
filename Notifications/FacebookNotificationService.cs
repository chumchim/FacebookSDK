using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FacebookSDK.Messages;
using FacebookSDK.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookSDK.Notifications;

/// <summary>
/// Facebook One-Time Notification Implementation
/// </summary>
public class FacebookNotificationService : IFacebookNotification
{
    private const string BaseUrl = "https://graph.facebook.com";

    private readonly HttpClient _httpClient;
    private readonly FacebookClientOptions _options;
    private readonly ILogger<FacebookNotificationService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public FacebookNotificationService(
        HttpClient httpClient,
        IOptions<FacebookClientOptions> options,
        ILogger<FacebookNotificationService>? logger = null)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<bool> SendRequestAsync(string recipientId, string title, string payload, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messages?access_token={_options.PageAccessToken}";

        var requestPayload = new
        {
            recipient = new { id = recipientId },
            message = new
            {
                attachment = new
                {
                    type = "template",
                    payload = new
                    {
                        template_type = "one_time_notif_req",
                        title = title,
                        payload = payload
                    }
                }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, requestPayload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error sending notification request: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to send notification request to {RecipientId}", recipientId);
            return false;
        }
    }

    public Task<bool> SendAsync(string token, string message, CancellationToken ct = default)
        => SendAsync(token, new TextMessage(message), ct);

    public async Task<bool> SendAsync(string token, IFacebookMessage message, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messages?access_token={_options.PageAccessToken}";

        var payload = new
        {
            recipient = new { one_time_notif_token = token },
            message = message.ToJson()
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error sending notification: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to send notification with token");
            return false;
        }
    }
}
