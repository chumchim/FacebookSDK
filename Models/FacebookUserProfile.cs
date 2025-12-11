using System.Text.Json.Serialization;

namespace FacebookSDK.Models;

/// <summary>
/// Facebook User Profile
/// </summary>
public record FacebookUserProfile
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    [JsonPropertyName("profile_pic")]
    public string? ProfilePic { get; init; }

    [JsonPropertyName("locale")]
    public string? Locale { get; init; }

    [JsonPropertyName("timezone")]
    public int? Timezone { get; init; }

    [JsonPropertyName("gender")]
    public string? Gender { get; init; }

    /// <summary>
    /// Full name (FirstName + LastName)
    /// </summary>
    [JsonIgnore]
    public string DisplayName => $"{FirstName} {LastName}".Trim();
}
