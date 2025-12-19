using System.Net.Http.Json;
using System.Text.Json;
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

    #region Attachment Messages

    public Task SendImageAsync(string imageUrl, string recipientId, CancellationToken ct = default)
        => SendAsync(new ImageMessage(imageUrl), recipientId, ct);

    public Task SendVideoAsync(string videoUrl, string recipientId, CancellationToken ct = default)
        => SendAsync(new VideoMessage(videoUrl), recipientId, ct);

    public Task SendAudioAsync(string audioUrl, string recipientId, CancellationToken ct = default)
        => SendAsync(new AudioMessage(audioUrl), recipientId, ct);

    public Task SendFileAsync(string fileUrl, string recipientId, CancellationToken ct = default)
        => SendAsync(new FileMessage(fileUrl), recipientId, ct);

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

    public Task SendWithTagAsync(IFacebookMessage message, string recipientId, MessageTagType tag, CancellationToken ct = default)
        => SendWithTagAsync(message, recipientId, tag.ToApiString(), ct);

    public Task SendWithTagAsync(string message, string recipientId, MessageTagType tag, CancellationToken ct = default)
        => SendWithTagAsync(new TextMessage(message), recipientId, tag.ToApiString(), ct);

    #endregion

    #region Persona Messages

    public async Task SendWithPersonaAsync(IFacebookMessage message, string recipientId, string personaId, CancellationToken ct = default)
    {
        var payload = new
        {
            recipient = new { id = recipientId },
            message = message.ToJson(),
            persona_id = personaId
        };

        await SendToGraphApiAsync(payload, ct);
    }

    public Task SendWithPersonaAsync(string message, string recipientId, string personaId, CancellationToken ct = default)
        => SendWithPersonaAsync(new TextMessage(message), recipientId, personaId, ct);

    #endregion

    #region Fallback Methods

    // Facebook error subcodes
    private const int ErrorSubcode_OutsideWindow = 2018278; // Cannot message user outside 24h window
    private const int ErrorSubcode_HumanAgentNotApproved = 2018276; // HUMAN_AGENT tag not approved

    public Task<FacebookSendResult> SendWithHumanAgentFallbackAsync(string message, string recipientId, CancellationToken ct = default)
        => SendWithHumanAgentFallbackAsync(new TextMessage(message), recipientId, ct);

    public async Task<FacebookSendResult> SendWithHumanAgentFallbackAsync(IFacebookMessage message, string recipientId, CancellationToken ct = default)
    {
        // Step 1: Try standard messaging first (within 24h window)
        var standardPayload = new
        {
            recipient = new { id = recipientId },
            message = message.ToJson()
        };

        var (success, errorResponse, statusCode) = await TrySendToGraphApiAsync(standardPayload, ct);

        if (success)
        {
            _logger?.LogDebug("Facebook message sent successfully using standard messaging");
            return FacebookSendResult.Ok(FacebookSendMethod.Standard);
        }

        // Parse error to check if we need HUMAN_AGENT tag
        var errorInfo = ParseFacebookError(errorResponse);

        // If error is NOT about messaging window, fail immediately
        if (!IsOutsideMessagingWindowError(errorInfo))
        {
            _logger?.LogWarning("Facebook standard messaging failed with non-window error: {Error}", errorResponse);
            return FacebookSendResult.Failed(errorInfo.Message ?? "Failed to send message");
        }

        _logger?.LogInformation("Standard messaging failed (outside 24h window), trying HUMAN_AGENT tag...");

        // Step 2: Try with HUMAN_AGENT tag
        var taggedPayload = new
        {
            recipient = new { id = recipientId },
            message = message.ToJson(),
            messaging_type = "MESSAGE_TAG",
            tag = "HUMAN_AGENT"
        };

        (success, errorResponse, statusCode) = await TrySendToGraphApiAsync(taggedPayload, ct);

        if (success)
        {
            _logger?.LogInformation("Facebook message sent successfully using HUMAN_AGENT tag");
            return FacebookSendResult.Ok(FacebookSendMethod.HumanAgentTag);
        }

        // Parse the HUMAN_AGENT error
        errorInfo = ParseFacebookError(errorResponse);

        // Check if HUMAN_AGENT permission is denied
        if (IsHumanAgentPermissionDenied(errorInfo))
        {
            _logger?.LogError(
                "HUMAN_AGENT tag permission denied. Please request 'human_agent' permission in Facebook App Review. Error: {Error}",
                errorResponse);
            return FacebookSendResult.Failed(
                "HUMAN_AGENT tag requires Facebook approval. Please request 'human_agent' permission in App Review.",
                humanAgentRequired: true,
                humanAgentDenied: true);
        }

        _logger?.LogError("Facebook HUMAN_AGENT messaging failed: {Error}", errorResponse);
        return FacebookSendResult.Failed(
            errorInfo.Message ?? "Failed to send message with HUMAN_AGENT tag",
            humanAgentRequired: true);
    }

    #endregion

    #region Private Methods

    private async Task SendToGraphApiAsync(object payload, CancellationToken ct)
    {
        // Use PageId instead of "me" for Page Access Tokens
        var pageId = _options.PageId ?? throw new InvalidOperationException("Facebook PageId is required for sending messages");
        var url = $"{BaseUrl}/{_options.ApiVersion}/{pageId}/messages?access_token={_options.PageAccessToken}";

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

    /// <summary>
    /// Try to send to Graph API, returning success/failure without throwing
    /// </summary>
    private async Task<(bool Success, string? ErrorResponse, System.Net.HttpStatusCode StatusCode)> TrySendToGraphApiAsync(
        object payload,
        CancellationToken ct)
    {
        var pageId = _options.PageId ?? throw new InvalidOperationException("Facebook PageId is required for sending messages");
        var url = $"{BaseUrl}/{_options.ApiVersion}/{pageId}/messages?access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (response.IsSuccessStatusCode)
            {
                return (true, null, response.StatusCode);
            }

            var error = await response.Content.ReadAsStringAsync(ct);
            return (false, error, response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Facebook API Request failed");
            return (false, ex.Message, System.Net.HttpStatusCode.ServiceUnavailable);
        }
    }

    /// <summary>
    /// Parse Facebook error response to extract error details
    /// </summary>
    private static FacebookErrorInfo ParseFacebookError(string? errorResponse)
    {
        if (string.IsNullOrEmpty(errorResponse))
            return new FacebookErrorInfo();

        try
        {
            using var doc = JsonDocument.Parse(errorResponse);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                return new FacebookErrorInfo
                {
                    Code = errorElement.TryGetProperty("code", out var code) ? code.GetInt32() : 0,
                    Subcode = errorElement.TryGetProperty("error_subcode", out var subcode) ? subcode.GetInt32() : 0,
                    Message = errorElement.TryGetProperty("message", out var msg) ? msg.GetString() : null,
                    Type = errorElement.TryGetProperty("type", out var type) ? type.GetString() : null
                };
            }
        }
        catch (JsonException)
        {
            // Ignore JSON parsing errors
        }

        return new FacebookErrorInfo { Message = errorResponse };
    }

    /// <summary>
    /// Check if error indicates message is outside 24h window
    /// </summary>
    private static bool IsOutsideMessagingWindowError(FacebookErrorInfo error)
    {
        // Error code 10 with subcode 2018278 = outside messaging window
        // Also check for code 100 which can indicate policy violations
        if (error.Subcode == ErrorSubcode_OutsideWindow)
            return true;

        // Some errors about "cannot message" or "24 hours" in message text
        if (error.Message?.Contains("24", StringComparison.OrdinalIgnoreCase) == true ||
            error.Message?.Contains("window", StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    }

    /// <summary>
    /// Check if error indicates HUMAN_AGENT permission is denied
    /// </summary>
    private static bool IsHumanAgentPermissionDenied(FacebookErrorInfo error)
    {
        // Error subcode 2018276 = HUMAN_AGENT not approved
        return error.Subcode == ErrorSubcode_HumanAgentNotApproved ||
               error.Message?.Contains("HUMAN_AGENT", StringComparison.OrdinalIgnoreCase) == true;
    }

    #endregion
}

/// <summary>
/// Parsed Facebook error information
/// </summary>
internal class FacebookErrorInfo
{
    public int Code { get; init; }
    public int Subcode { get; init; }
    public string? Message { get; init; }
    public string? Type { get; init; }
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
