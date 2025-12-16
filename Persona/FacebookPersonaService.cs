using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FacebookSDK.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookSDK.Persona;

/// <summary>
/// Facebook Persona API Implementation
/// </summary>
public class FacebookPersonaService : IFacebookPersona
{
    private const string BaseUrl = "https://graph.facebook.com";

    private readonly HttpClient _httpClient;
    private readonly FacebookClientOptions _options;
    private readonly ILogger<FacebookPersonaService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public FacebookPersonaService(
        HttpClient httpClient,
        IOptions<FacebookClientOptions> options,
        ILogger<FacebookPersonaService>? logger = null)
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

    public async Task<PersonaInfo> CreateAsync(string name, string profilePictureUrl, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/personas?access_token={_options.PageAccessToken}";

        var payload = new
        {
            name,
            profile_picture_url = profilePictureUrl
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error creating persona: {StatusCode} - {Error}", response.StatusCode, error);
                throw new FacebookPersonaException($"Failed to create persona: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<CreatePersonaResponse>(_jsonOptions, ct);

            return new PersonaInfo
            {
                Id = result?.Id ?? "",
                Name = name,
                ProfilePictureUrl = profilePictureUrl
            };
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to create persona");
            throw;
        }
    }

    public async Task<PersonaInfo?> GetAsync(string personaId, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/{personaId}?fields=id,name,profile_picture_url&access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error getting persona: {StatusCode} - {Error}", response.StatusCode, error);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PersonaInfo>(_jsonOptions, ct);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get persona {PersonaId}", personaId);
            return null;
        }
    }

    public async Task<List<PersonaInfo>> GetAllAsync(CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/me/personas?access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error getting personas: {StatusCode} - {Error}", response.StatusCode, error);
                return new List<PersonaInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<PersonaListResponse>(_jsonOptions, ct);
            return result?.Data ?? new List<PersonaInfo>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get all personas");
            return new List<PersonaInfo>();
        }
    }

    public async Task<bool> DeleteAsync(string personaId, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{_options.ApiVersion}/{personaId}?access_token={_options.PageAccessToken}";

        try
        {
            var response = await _httpClient.DeleteAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger?.LogError("Facebook API Error deleting persona: {StatusCode} - {Error}", response.StatusCode, error);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<DeleteResponse>(_jsonOptions, ct);
            return result?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to delete persona {PersonaId}", personaId);
            return false;
        }
    }

    private class CreatePersonaResponse
    {
        public string Id { get; set; } = string.Empty;
    }

    private class PersonaListResponse
    {
        public List<PersonaInfo> Data { get; set; } = new();
    }

    private class DeleteResponse
    {
        public bool Success { get; set; }
    }
}

/// <summary>
/// Persona API Exception
/// </summary>
public class FacebookPersonaException : Exception
{
    public FacebookPersonaException(string message) : base(message) { }
    public FacebookPersonaException(string message, Exception innerException) : base(message, innerException) { }
}
