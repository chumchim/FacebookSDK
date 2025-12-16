using System.Net.Http.Json;
using FacebookSDK.Models;
using FacebookSDK.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookSDK.Profile;

/// <summary>
/// Facebook Profile Service Implementation
/// </summary>
public class FacebookProfileService : IFacebookProfile
{
    private const string BaseUrl = "https://graph.facebook.com";
    private static readonly string[] DefaultFields = { "first_name", "last_name", "profile_pic" };

    private readonly HttpClient _httpClient;
    private readonly FacebookClientOptions _options;
    private readonly ILogger<FacebookProfileService>? _logger;

    public FacebookProfileService(
        HttpClient httpClient,
        IOptions<FacebookClientOptions> options,
        ILogger<FacebookProfileService>? logger = null)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task<FacebookUserProfile?> GetUserProfileAsync(string userId, CancellationToken ct = default)
        => GetUserProfileAsync(userId, DefaultFields, ct);

    public async Task<FacebookUserProfile?> GetUserProfileAsync(string userId, string[] fields, CancellationToken ct = default)
    {
        var fieldsParam = string.Join(",", fields);
        var url = $"{BaseUrl}/{_options.ApiVersion}/{userId}?fields={fieldsParam}&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogWarning(
                    "Failed to get profile for user {UserId}: {StatusCode}. Error: {Error}",
                    userId, response.StatusCode, errorBody);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<FacebookUserProfile>(ct);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting profile for user {UserId}", userId);
            return null;
        }
    }
}
