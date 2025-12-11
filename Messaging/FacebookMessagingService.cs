using System.Net.Http.Json;
using FacebookSDK.Messages;
using FacebookSDK.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookSDK.Messaging;

/// <summary>
/// Facebook Messaging Service Implementation
/// </summary>
public class FacebookMessagingService : IFacebookMessaging
{
    private const string BaseUrl = "https://graph.facebook.com";

    private readonly HttpClient _httpClient;
    private readonly FacebookClientOptions _options;
    private readonly ILogger<FacebookMessagingService>? _logger;

    public FacebookMessagingService(
        HttpClient httpClient,
        IOptions<FacebookClientOptions> options,
        ILogger<FacebookMessagingService>? logger = null)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    #region Send Message

    public Task SendAsync(string message, string recipientId, CancellationToken ct = default)
        => SendAsync(new TextMessage(message), recipientId, ct);

    public async Task SendAsync(IFacebookMessage message, string recipientId, CancellationToken ct = default)
    {
        var payload = new
        {
            recipient = new { id = recipientId },
            message = message.ToJson()
        };

        await SendToGraphApiAsync(payload, ct);
    }

    #endregion

    #region Sender Actions

    public Task SendTypingOnAsync(string recipientId, CancellationToken ct = default)
        => SendSenderActionAsync(recipientId, "typing_on", ct);

    public Task SendTypingOffAsync(string recipientId, CancellationToken ct = default)
        => SendSenderActionAsync(recipientId, "typing_off", ct);

    public Task SendMarkSeenAsync(string recipientId, CancellationToken ct = default)
        => SendSenderActionAsync(recipientId, "mark_seen", ct);

    private async Task SendSenderActionAsync(string recipientId, string action, CancellationToken ct)
    {
        var payload = new
        {
            recipient = new { id = recipientId },
            sender_action = action
        };

        await SendToGraphApiAsync(payload, ct);
    }

    #endregion

    #region Message Tags

    public async Task SendWithTagAsync(IFacebookMessage message, string recipientId, string tag, CancellationToken ct = default)
    {
        var payload = new
        {
            recipient = new { id = recipientId },
            message = message.ToJson(),
            messaging_type = "MESSAGE_TAG",
            tag
        };

        await SendToGraphApiAsync(payload, ct);
    }

    #endregion

    #region Private Methods

    private async Task SendToGraphApiAsync(object payload, CancellationToken ct)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messages?access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error: {StatusCode} - {Error}", response.StatusCode, error);
                throw new FacebookApiException(response.StatusCode, error);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Facebook API Request failed");
            throw;
        }
    }

    #endregion
}

/// <summary>
/// Facebook API Exception
/// </summary>
public class FacebookApiException : Exception
{
    public System.Net.HttpStatusCode StatusCode { get; }
    public string ErrorResponse { get; }

    public FacebookApiException(System.Net.HttpStatusCode statusCode, string errorResponse)
        : base($"Facebook API Error: {statusCode} - {errorResponse}")
    {
        StatusCode = statusCode;
        ErrorResponse = errorResponse;
    }
}
