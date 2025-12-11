using System.Text.Json.Serialization;

namespace FacebookSDK.Webhook;

/// <summary>
/// Facebook Message (Webhook)
/// </summary>
public record FacebookMessage
{
    [JsonPropertyName("mid")]
    public string Mid { get; init; } = "";

    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonPropertyName("attachments")]
    public List<FacebookAttachment>? Attachments { get; init; }

    [JsonPropertyName("quick_reply")]
    public FacebookQuickReplyPayload? QuickReply { get; init; }

    [JsonPropertyName("reply_to")]
    public FacebookReplyTo? ReplyTo { get; init; }

    #region Helper Properties

    [JsonIgnore]
    public bool HasText => !string.IsNullOrEmpty(Text);

    [JsonIgnore]
    public bool HasAttachments => Attachments?.Any() == true;

    [JsonIgnore]
    public bool IsQuickReply => QuickReply != null;

    #endregion
}

/// <summary>
/// Facebook Attachment
/// </summary>
public record FacebookAttachment
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "";

    [JsonPropertyName("payload")]
    public FacebookAttachmentPayload? Payload { get; init; }

    #region Helper Properties

    [JsonIgnore]
    public bool IsImage => Type.Equals("image", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public bool IsVideo => Type.Equals("video", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public bool IsAudio => Type.Equals("audio", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public bool IsFile => Type.Equals("file", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public bool IsLocation => Type.Equals("location", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public bool IsFallback => Type.Equals("fallback", StringComparison.OrdinalIgnoreCase);

    #endregion
}

/// <summary>
/// Facebook Attachment Payload
/// </summary>
public record FacebookAttachmentPayload
{
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("sticker_id")]
    public long? StickerId { get; init; }

    [JsonPropertyName("coordinates")]
    public FacebookCoordinates? Coordinates { get; init; }
}

/// <summary>
/// Facebook Coordinates (for location)
/// </summary>
public record FacebookCoordinates
{
    [JsonPropertyName("lat")]
    public double Latitude { get; init; }

    [JsonPropertyName("long")]
    public double Longitude { get; init; }
}

/// <summary>
/// Facebook Quick Reply Payload
/// </summary>
public record FacebookQuickReplyPayload
{
    [JsonPropertyName("payload")]
    public string Payload { get; init; } = "";
}

/// <summary>
/// Facebook Reply To
/// </summary>
public record FacebookReplyTo
{
    [JsonPropertyName("mid")]
    public string Mid { get; init; } = "";
}

/// <summary>
/// Facebook Postback
/// </summary>
public record FacebookPostback
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("payload")]
    public string Payload { get; init; } = "";

    [JsonPropertyName("referral")]
    public FacebookReferral? Referral { get; init; }
}

/// <summary>
/// Facebook Referral
/// </summary>
public record FacebookReferral
{
    [JsonPropertyName("ref")]
    public string? Ref { get; init; }

    [JsonPropertyName("source")]
    public string? Source { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }
}

/// <summary>
/// Facebook Delivery
/// </summary>
public record FacebookDelivery
{
    [JsonPropertyName("mids")]
    public List<string>? Mids { get; init; }

    [JsonPropertyName("watermark")]
    public long Watermark { get; init; }
}

/// <summary>
/// Facebook Read
/// </summary>
public record FacebookRead
{
    [JsonPropertyName("watermark")]
    public long Watermark { get; init; }
}
