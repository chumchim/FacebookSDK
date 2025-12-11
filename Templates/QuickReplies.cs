using FacebookSDK.Messages;

namespace FacebookSDK.Templates;

/// <summary>
/// Quick Replies - ปุ่มตอบกลับด่วน
/// </summary>
public record QuickReplies : IFacebookMessage
{
    public IFacebookMessage Message { get; init; }
    public List<QuickReply> Replies { get; init; }

    public QuickReplies(string text, List<QuickReply> replies)
    {
        Message = new TextMessage(text);
        Replies = replies;
    }

    public QuickReplies(IFacebookMessage message, List<QuickReply> replies)
    {
        Message = message;
        Replies = replies;
    }

    public object ToJson()
    {
        var messageJson = Message.ToJson();
        var dict = messageJson as IDictionary<string, object>
                   ?? new Dictionary<string, object>(
                       messageJson.GetType().GetProperties()
                           .ToDictionary(p => p.Name, p => p.GetValue(messageJson)!));

        return new
        {
            message = dict,
            quick_replies = Replies.Select(r => r.ToJson())
        };
    }
}

/// <summary>
/// Quick Reply Button
/// </summary>
public record QuickReply
{
    public string ContentType { get; init; } = "text";
    public string? Title { get; init; }
    public string? Payload { get; init; }
    public string? ImageUrl { get; init; }

    /// <summary>
    /// Text Quick Reply
    /// </summary>
    public static QuickReply Text(string title, string payload, string? imageUrl = null) => new()
    {
        ContentType = "text",
        Title = title,
        Payload = payload,
        ImageUrl = imageUrl
    };

    /// <summary>
    /// User Phone Number Quick Reply
    /// </summary>
    public static QuickReply UserPhoneNumber => new() { ContentType = "user_phone_number" };

    /// <summary>
    /// User Email Quick Reply
    /// </summary>
    public static QuickReply UserEmail => new() { ContentType = "user_email" };

    public object ToJson()
    {
        var obj = new Dictionary<string, object>
        {
            ["content_type"] = ContentType
        };

        if (!string.IsNullOrEmpty(Title))
            obj["title"] = Title;

        if (!string.IsNullOrEmpty(Payload))
            obj["payload"] = Payload;

        if (!string.IsNullOrEmpty(ImageUrl))
            obj["image_url"] = ImageUrl;

        return obj;
    }
}
