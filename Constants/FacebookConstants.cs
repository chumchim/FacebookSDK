namespace FacebookSDK.Constants;

/// <summary>
/// Facebook Webhook Event Types
/// </summary>
public static class FacebookEventTypes
{
    public const string Message = "message";
    public const string Postback = "postback";
    public const string Referral = "referral";
    public const string Delivery = "delivery";
    public const string Read = "read";
    public const string Echo = "echo";
    public const string Optin = "optin";
    public const string PolicyEnforcement = "policy_enforcement";
}

/// <summary>
/// Facebook Attachment Types
/// </summary>
public static class FacebookAttachmentTypes
{
    public const string Image = "image";
    public const string Video = "video";
    public const string Audio = "audio";
    public const string File = "file";
    public const string Location = "location";
    public const string Fallback = "fallback";
    public const string Template = "template";
}

/// <summary>
/// Facebook Template Types
/// </summary>
public static class FacebookTemplateTypes
{
    public const string Generic = "generic";
    public const string Button = "button";
    public const string Media = "media";
    public const string Receipt = "receipt";
    public const string Airline = "airline";
}

/// <summary>
/// Facebook Button Types
/// </summary>
public static class FacebookButtonTypes
{
    public const string WebUrl = "web_url";
    public const string Postback = "postback";
    public const string PhoneNumber = "phone_number";
    public const string Login = "account_link";
    public const string Logout = "account_unlink";
    public const string GamePlay = "game_play";
}

/// <summary>
/// Facebook Webhook Signature
/// </summary>
public static class FacebookSignature
{
    public const string Prefix = "sha256=";
    public const string HeaderName = "X-Hub-Signature-256";
}

/// <summary>
/// Facebook Webhook Object Types
/// </summary>
public static class FacebookObjectTypes
{
    public const string Page = "page";
    public const string Instagram = "instagram";
}
