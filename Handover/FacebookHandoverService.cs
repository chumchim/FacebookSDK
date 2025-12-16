using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FacebookSDK.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookSDK.Handover;

/// <summary>
/// Facebook Handover Protocol Implementation
/// </summary>
public class FacebookHandoverService : IFacebookHandover
{
    private const string BaseUrl = "https://graph.facebook.com";

    private readonly HttpClient _httpClient;
    private readonly FacebookClientOptions _options;
    private readonly ILogger<FacebookHandoverService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public FacebookHandoverService(
        HttpClient httpClient,
        IOptions<FacebookClientOptions> options,
        ILogger<FacebookHandoverService>? logger = null)
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

    public Task<bool> PassThreadControlAsync(string recipientId, HandoverTarget target, string? metadata = null, CancellationToken ct = default)
    {
        var targetAppId = target switch
        {
            HandoverTarget.PageInbox => HandoverAppIds.PageInbox,
            HandoverTarget.PrimaryReceiver => throw new ArgumentException("Use TakeThreadControlAsync to return to primary receiver"),
            _ => throw new ArgumentOutOfRangeException(nameof(target))
        };

        return PassThreadControlAsync(recipientId, targetAppId, metadata, ct);
    }

    public async Task<bool> PassThreadControlAsync(string recipientId, string targetAppId, string? metadata = null, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/pass_thread_control?access_token={_options.PageAccessToken}";

        var payload = new Dictionary<string, object>
        {
            ["recipient"] = new { id = recipientId },
            ["target_app_id"] = targetAppId
        };

        if (!string.IsNullOrEmpty(metadata))
        {
            payload["metadata"] = metadata;
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error passing thread control: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(_jsonOptions, ct);
            return result?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to pass thread control for {RecipientId}", recipientId);
            return false;
        }
    }

    public async Task<bool> TakeThreadControlAsync(string recipientId, string? metadata = null, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/take_thread_control?access_token={_options.PageAccessToken}";

        var payload = new Dictionary<string, object>
        {
            ["recipient"] = new { id = recipientId }
        };

        if (!string.IsNullOrEmpty(metadata))
        {
            payload["metadata"] = metadata;
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error taking thread control: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(_jsonOptions, ct);
            return result?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to take thread control for {RecipientId}", recipientId);
            return false;
        }
    }

    public async Task<bool> RequestThreadControlAsync(string recipientId, string? metadata = null, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/request_thread_control?access_token={_options.PageAccessToken}";

        var payload = new Dictionary<string, object>
        {
            ["recipient"] = new { id = recipientId }
        };

        if (!string.IsNullOrEmpty(metadata))
        {
            payload["metadata"] = metadata;
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error requesting thread control: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(_jsonOptions, ct);
            return result?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to request thread control for {RecipientId}", recipientId);
            return false;
        }
    }

    public async Task<ThreadOwnerInfo?> GetThreadOwnerAsync(string recipientId, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/thread_owner?recipient={recipientId}&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error getting thread owner: {StatusCode} - {Error}", response.StatusCode, error);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ThreadOwnerResponse>(_jsonOptions, ct);
            return result?.Data?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get thread owner for {RecipientId}", recipientId);
            return null;
        }
    }

    public async Task<List<SecondaryReceiverInfo>> GetSecondaryReceiversAsync(CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/secondary_receivers?fields=id,name&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error getting secondary receivers: {StatusCode} - {Error}", response.StatusCode, error);
                return new List<SecondaryReceiverInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<SecondaryReceiversResponse>(_jsonOptions, ct);
            return result?.Data ?? new List<SecondaryReceiverInfo>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get secondary receivers");
            return new List<SecondaryReceiverInfo>();
        }
    }

    private class SuccessResponse
    {
        public bool Success { get; set; }
    }

    private class ThreadOwnerResponse
    {
        public List<ThreadOwnerInfo>? Data { get; set; }
    }

    private class SecondaryReceiversResponse
    {
        public List<SecondaryReceiverInfo>? Data { get; set; }
    }
}
