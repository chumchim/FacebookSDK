namespace FacebookSDK.Messages;

/// <summary>
/// Facebook Message Interface
/// </summary>
public interface IFacebookMessage
{
    /// <summary>
    /// Convert to JSON object for API
    /// </summary>
    object ToJson();
}

/// <summary>
/// Text Message
/// </summary>
public record TextMessage(string Text) : IFacebookMessage
{
    public object ToJson() => new { text = Text };
}

/// <summary>
/// Image Message
/// </summary>
public record ImageMessage(string Url) : IFacebookMessage
{
    public object ToJson() => new
    {
        attachment = new
        {
            type = "image",
            payload = new { url = Url, is_reusable = true }
        }
    };
}

/// <summary>
/// Video Message
/// </summary>
public record VideoMessage(string Url) : IFacebookMessage
{
    public object ToJson() => new
    {
        attachment = new
        {
            type = "video",
            payload = new { url = Url, is_reusable = true }
        }
    };
}

/// <summary>
/// Audio Message
/// </summary>
public record AudioMessage(string Url) : IFacebookMessage
{
    public object ToJson() => new
    {
        attachment = new
        {
            type = "audio",
            payload = new { url = Url, is_reusable = true }
        }
    };
}

/// <summary>
/// File Message
/// </summary>
public record FileMessage(string Url) : IFacebookMessage
{
    public object ToJson() => new
    {
        attachment = new
        {
            type = "file",
            payload = new { url = Url, is_reusable = true }
        }
    };
}
