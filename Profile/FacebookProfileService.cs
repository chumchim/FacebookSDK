using System.Net.Http.Json;
using System.Text.Json;
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

    /// <summary>
    /// ดึง profile ของ user ผ่าน Conversations API (ไม่ต้อง App Review)
    /// </summary>
    public async Task<FacebookUserProfile?> GetUserProfileViaConversationsAsync(string psid, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/conversations?fields=participants&user_id={psid}&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogWarning("Failed to get profile via conversations for PSID {Psid}: {StatusCode}. Error: {Error}",
                    psid, response.StatusCode, errorBody);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var conversationsResponse = JsonSerializer.Deserialize<ConversationsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (conversationsResponse?.Data == null || conversationsResponse.Data.Count == 0)
                return null;

            // Find the participant that matches the PSID (not the page)
            var participant = conversationsResponse.Data
                .SelectMany(c => c.Participants?.Data ?? [])
                .FirstOrDefault(p => p.Id == psid);

            if (participant == null)
                return null;

            return new FacebookUserProfile
            {
                Id = participant.Id ?? "",
                FirstName = participant.Name, // Conversations API returns full name only
                LastName = null,
                ProfilePic = null // Not available via Conversations API
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting profile via conversations for PSID {Psid}", psid);
            return null;
        }
    }

    // Response models for Conversations API
    private class ConversationsResponse
    {
        public List<ConversationData>? Data { get; set; }
    }

    private class ConversationData
    {
        public ParticipantsData? Participants { get; set; }
        public string? Id { get; set; }
    }

    private class ParticipantsData
    {
        public List<ParticipantInfo>? Data { get; set; }
    }

    private class ParticipantInfo
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
    }
}
