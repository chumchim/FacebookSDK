namespace FacebookSDK.Options;

/// <summary>
/// Facebook Client Options
/// </summary>
public class FacebookClientOptions
{
    public const string SectionName = "Facebook";

    /// <summary>
    /// Page Access Token (required)
    /// </summary>
    public string PageAccessToken { get; set; } = "";

    /// <summary>
    /// App Secret สำหรับ verify webhook signature (required)
    /// </summary>
    public string AppSecret { get; set; } = "";

    /// <summary>
    /// Verify Token สำหรับ webhook subscription (required)
    /// </summary>
    public string VerifyToken { get; set; } = "";

    /// <summary>
    /// Page ID (optional - สำหรับ reference)
    /// </summary>
    public string? PageId { get; set; }

    /// <summary>
    /// App ID (optional - สำหรับ reference)
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// API Version (default: v18.0)
    /// </summary>
    public string ApiVersion { get; set; } = "v18.0";

    /// <summary>
    /// Validate options
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PageAccessToken))
            throw new InvalidOperationException("Facebook PageAccessToken is required");

        if (string.IsNullOrWhiteSpace(AppSecret))
            throw new InvalidOperationException("Facebook AppSecret is required");

        if (string.IsNullOrWhiteSpace(VerifyToken))
            throw new InvalidOperationException("Facebook VerifyToken is required");
    }
}
