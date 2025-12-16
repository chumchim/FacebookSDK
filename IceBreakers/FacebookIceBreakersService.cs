using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FacebookSDK.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookSDK.IceBreakers;

/// <summary>
/// Facebook Ice Breakers Implementation
/// </summary>
public class FacebookIceBreakersService : IFacebookIceBreakers
{
    private const string BaseUrl = "https://graph.facebook.com";

    private readonly HttpClient _httpClient;
    private readonly FacebookClientOptions _options;
    private readonly ILogger<FacebookIceBreakersService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public FacebookIceBreakersService(
        HttpClient httpClient,
        IOptions<FacebookClientOptions> options,
        ILogger<FacebookIceBreakersService>? logger = null)
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

    public async Task<bool> SetAsync(List<IceBreaker> iceBreakers, CancellationToken ct = default)
    {
        if (iceBreakers.Count > 4)
        {
            _logger?.LogWarning("Ice breakers limited to 4, truncating list");
            iceBreakers = iceBreakers.Take(4).ToList();
        }

        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messenger_profile?access_token={_options.PageAccessToken}";

        var payload = new
        {
            ice_breakers = iceBreakers.Select(ib => new
            {
                question = ib.Question,
                payload = ib.Payload
            }).ToList()
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error setting ice breakers: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ResultResponse>(_jsonOptions, ct);
            return result?.Result == "success";
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to set ice breakers");
            return false;
        }
    }

    public async Task<List<IceBreaker>> GetAsync(CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messenger_profile?fields=ice_breakers&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error getting ice breakers: {StatusCode} - {Error}", response.StatusCode, error);
                return new List<IceBreaker>();
            }

            var result = await response.Content.ReadFromJsonAsync<GetIceBreakersResponse>(_jsonOptions, ct);
            return result?.Data?.FirstOrDefault()?.IceBreakers ?? new List<IceBreaker>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get ice breakers");
            return new List<IceBreaker>();
        }
    }

    public async Task<bool> DeleteAsync(CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/messenger_profile?access_token={_options.PageAccessToken}";

        var payload = new
        {
            fields = new[] { "ice_breakers" }
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
                _logger?.LogError("Facebook API Error deleting ice breakers: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ResultResponse>(_jsonOptions, ct);
            return result?.Result == "success";
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to delete ice breakers");
            return false;
        }
    }

    private class ResultResponse
    {
        public string? Result { get; set; }
    }

    private class GetIceBreakersResponse
    {
        public List<IceBreakersData>? Data { get; set; }
    }

    private class IceBreakersData
    {
        public List<IceBreaker>? IceBreakers { get; set; }
    }
}
