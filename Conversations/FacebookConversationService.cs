using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FacebookSDK.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookSDK.Conversations;

/// <summary>
/// Facebook Conversations API Implementation
/// </summary>
public class FacebookConversationService : IFacebookConversation
{
    private const string BaseUrl = "https://graph.facebook.com";

    private readonly HttpClient _httpClient;
    private readonly FacebookClientOptions _options;
    private readonly ILogger<FacebookConversationService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public FacebookConversationService(
        HttpClient httpClient,
        IOptions<FacebookClientOptions> options,
        ILogger<FacebookConversationService>? logger = null)
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

    public Task<ConversationListResponse> GetConversationsAsync(int limit = 20, CancellationToken ct = default)
        => GetConversationsAsync(null!, limit, ct);

    public async Task<ConversationListResponse> GetConversationsAsync(string afterCursor, int limit = 20, CancellationToken ct = default)
    {
        var pageId = _options.PageId ?? "me";
        var fields = "id,link,updated_time,participants,unread_count";
        var url = $"{BaseUrl}/{_options.ApiVersion}/{pageId}/conversations?fields={fields}&limit={limit}&access_token={_options.PageAccessToken}";

        if (!string.IsNullOrEmpty(afterCursor))
        {
            url += $"&after={afterCursor}";
        }

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error: {StatusCode} - {Error}", response.StatusCode, error);
                return new ConversationListResponse();
            }

            var result = await response.Content.ReadFromJsonAsync<ConversationListResponse>(_jsonOptions, ct);
            return result ?? new ConversationListResponse();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get conversations");
            return new ConversationListResponse();
        }
    }

    public Task<MessageListResponse> GetMessagesAsync(string conversationId, int limit = 20, CancellationToken ct = default)
        => GetMessagesAsync(conversationId, null!, limit, ct);

    public async Task<MessageListResponse> GetMessagesAsync(string conversationId, string afterCursor, int limit = 20, CancellationToken ct = default)
    {
        var fields = "id,message,from,to,created_time,attachments,sticker,shares,tags";
        var url = $"{BaseUrl}/{_options.ApiVersion}/{conversationId}/messages?fields={fields}&limit={limit}&access_token={_options.PageAccessToken}";

        if (!string.IsNullOrEmpty(afterCursor))
        {
            url += $"&after={afterCursor}";
        }

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error: {StatusCode} - {Error}", response.StatusCode, error);
                return new MessageListResponse();
            }

            var result = await response.Content.ReadFromJsonAsync<MessageListResponse>(_jsonOptions, ct);
            return result ?? new MessageListResponse();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get messages for conversation {ConversationId}", conversationId);
            return new MessageListResponse();
        }
    }

    public async Task<ConversationMessage?> GetMessageAsync(string messageId, CancellationToken ct = default)
    {
        var fields = "id,message,from,to,created_time,attachments,sticker,shares,tags";
        var url = $"{BaseUrl}/{_options.ApiVersion}/{messageId}?fields={fields}&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error: {StatusCode} - {Error}", response.StatusCode, error);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ConversationMessage>(_jsonOptions, ct);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get message {MessageId}", messageId);
            return null;
        }
    }

    public async Task<Conversation?> GetConversationByPsidAsync(string psid, CancellationToken ct = default)
    {
        var pageId = _options.PageId ?? "me";
        var fields = "id,link,updated_time,participants,unread_count";
        var url = $"{BaseUrl}/{_options.ApiVersion}/{pageId}/conversations?user_id={psid}&fields={fields}&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error: {StatusCode} - {Error}", response.StatusCode, error);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ConversationListResponse>(_jsonOptions, ct);
            return result?.Data?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get conversation for PSID {Psid}", psid);
            return null;
        }
    }
}
