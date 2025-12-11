using System.Text.Json.Serialization;

namespace FacebookSDK.Webhook;

/// <summary>
/// Facebook Webhook Payload - โครงสร้างหลักที่ Facebook ส่งมา
/// </summary>
public record FacebookWebhookPayload
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = "";

    [JsonPropertyName("entry")]
    public List<FacebookEntry> Entry { get; init; } = new();
}

/// <summary>
/// Facebook Entry
/// </summary>
public record FacebookEntry
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("time")]
    public long Time { get; init; }

    [JsonPropertyName("messaging")]
    public List<FacebookMessagingEvent>? Messaging { get; init; }
}

/// <summary>
/// Facebook Messaging Event
/// </summary>
public record FacebookMessagingEvent
{
    [JsonPropertyName("sender")]
    public FacebookSender? Sender { get; init; }

    [JsonPropertyName("recipient")]
    public FacebookRecipient? Recipient { get; init; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; }

    [JsonPropertyName("message")]
    public FacebookMessage? Message { get; init; }

    [JsonPropertyName("postback")]
    public FacebookPostback? Postback { get; init; }

    [JsonPropertyName("delivery")]
    public FacebookDelivery? Delivery { get; init; }

    [JsonPropertyName("read")]
    public FacebookRead? Read { get; init; }

    #region Helper Properties

    [JsonIgnore]
    public bool IsMessageEvent => Message != null;

    [JsonIgnore]
    public bool IsPostbackEvent => Postback != null;

    [JsonIgnore]
    public bool IsDeliveryEvent => Delivery != null;

    [JsonIgnore]
    public bool IsReadEvent => Read != null;

    #endregion
}

/// <summary>
/// Facebook Sender
/// </summary>
public record FacebookSender
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";
}

/// <summary>
/// Facebook Recipient
/// </summary>
public record FacebookRecipient
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";
}
