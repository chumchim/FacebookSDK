using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FacebookSDK.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookSDK.Menu;

/// <summary>
/// Facebook Persistent Menu Implementation
/// </summary>
public class FacebookMenuService : IFacebookMenu
{
    private const string BaseUrl = "https://graph.facebook.com";

    private readonly HttpClient _httpClient;
    private readonly FacebookClientOptions _options;
    private readonly ILogger<FacebookMenuService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public FacebookMenuService(
        HttpClient httpClient,
        IOptions<FacebookClientOptions> options,
        ILogger<FacebookMenuService>? logger = null)
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

    public Task<bool> SetAsync(PersistentMenu menu, CancellationToken ct = default)
        => SetAsync(new List<PersistentMenu> { menu }, ct);

    public async Task<bool> SetAsync(List<PersistentMenu> menus, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messenger_profile?access_token={_options.PageAccessToken}";

        var payload = new
        {
            persistent_menu = menus.Select(m => new
            {
                locale = m.Locale,
                composer_input_disabled = m.ComposerInputDisabled,
                call_to_actions = m.CallToActions.Select(ConvertMenuItem).ToList()
            }).ToList()
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error setting menu: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ResultResponse>(_jsonOptions, ct);
            return result?.Result == "success";
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to set persistent menu");
            return false;
        }
    }

    public async Task<List<PersistentMenu>> GetAsync(CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messenger_profile?fields=persistent_menu&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error getting menu: {StatusCode} - {Error}", response.StatusCode, error);
                return new List<PersistentMenu>();
            }

            var result = await response.Content.ReadFromJsonAsync<GetMenuResponse>(_jsonOptions, ct);
            return result?.Data?.FirstOrDefault()?.PersistentMenu ?? new List<PersistentMenu>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get persistent menu");
            return new List<PersistentMenu>();
        }
    }

    public async Task<bool> DeleteAsync(CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messenger_profile?access_token={_options.PageAccessToken}";

        var payload = new
        {
            fields = new[] { "persistent_menu" }
        };

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = JsonContent.Create(payload)
            };

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error deleting menu: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ResultResponse>(_jsonOptions, ct);
            return result?.Result == "success";
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to delete persistent menu");
            return false;
        }
    }

    private object ConvertMenuItem(MenuItem item)
    {
        var result = new Dictionary<string, object>
        {
            ["type"] = item.Type,
            ["title"] = item.Title
        };

        if (!string.IsNullOrEmpty(item.Payload))
            result["payload"] = item.Payload;

        if (!string.IsNullOrEmpty(item.Url))
            result["url"] = item.Url;

        if (!string.IsNullOrEmpty(item.WebviewHeightRatio))
            result["webview_height_ratio"] = item.WebviewHeightRatio;

        if (item.MessengerExtensions.HasValue)
            result["messenger_extensions"] = item.MessengerExtensions.Value;

        if (!string.IsNullOrEmpty(item.FallbackUrl))
            result["fallback_url"] = item.FallbackUrl;

        if (!string.IsNullOrEmpty(item.WebviewShareButton))
            result["webview_share_button"] = item.WebviewShareButton;

        if (item.CallToActions != null && item.CallToActions.Count > 0)
            result["call_to_actions"] = item.CallToActions.Select(ConvertMenuItem).ToList();

        return result;
    }

    private class ResultResponse
    {
        public string? Result { get; set; }
    }

    private class GetMenuResponse
    {
        public List<MenuData>? Data { get; set; }
    }

    private class MenuData
    {
        public List<PersistentMenu>? PersistentMenu { get; set; }
    }
}
