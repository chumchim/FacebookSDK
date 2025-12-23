# FacebookSDK - Facebook Messenger Platform Wrapper

Custom SDK for Facebook Messenger Platform integration.

---

## API Requirements & Permissions

### App Review Required

```
⚠️ IMPORTANT: Most Messenger features require App Review approval!
```

Before your app can be used publicly, you must submit for App Review with:
- Video walkthrough showing feature usage
- Detailed description of use case
- Privacy policy URL

### Required Permissions

| Permission | Purpose | App Review |
|------------|---------|:----------:|
| `pages_messaging` | Send/receive messages | ✅ Required |
| `pages_messaging_subscriptions` | Subscription messaging | ✅ Required |
| `pages_manage_metadata` | Manage page settings | ✅ Required |
| `pages_read_engagement` | Read page data | ✅ Required |

### Permission Status Check

```csharp
// Before using messaging features, ensure permissions are granted
// Check in Facebook Developer Console > App Review > Permissions
```

---

## 24-Hour Messaging Window

### Standard Messaging Policy

```
┌─────────────────────────────────────────────────────────────────┐
│                    24-HOUR MESSAGING WINDOW                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  User sends message                                             │
│        ↓                                                        │
│  ┌──────────────────────────────────────────────────────┐       │
│  │     24-HOUR WINDOW STARTS                            │       │
│  │     ✅ Can send unlimited messages                    │       │
│  │     ✅ Can send any content type                      │       │
│  └──────────────────────────────────────────────────────┘       │
│        ↓ (after 24 hours)                                       │
│  ┌──────────────────────────────────────────────────────┐       │
│  │     WINDOW EXPIRED                                    │       │
│  │     ❌ Cannot send regular messages                   │       │
│  │     ⚠️ Must use Message Tags                          │       │
│  └──────────────────────────────────────────────────────┘       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### Message Tags (For Messaging Outside 24-Hour Window)

| Tag | Use Case | Notes |
|-----|----------|-------|
| `CONFIRMED_EVENT_UPDATE` | Event reminders | User-initiated events only |
| `POST_PURCHASE_UPDATE` | Order updates | Shipping, delivery status |
| `ACCOUNT_UPDATE` | Account changes | Payment, settings changes |
| `HUMAN_AGENT` | Live agent support | **7-day window, requires approval** |

### HUMAN_AGENT Tag

```
⚠️ IMPORTANT: HUMAN_AGENT tag extends window to 7 days but requires:
   1. App Review approval for HUMAN_AGENT permission
   2. Only for human agent conversations (not bots)
   3. Must be real human interaction
```

---

## Service Reference

### IFacebookMessaging (Messaging Service)

```csharp
public interface IFacebookMessaging
{
    // ✅ Standard messaging (within 24-hour window)
    Task SendAsync(string message, string recipientId);
    Task SendAsync(IFacebookMessage message, string recipientId);
    Task SendImageAsync(string imageUrl, string recipientId);
    Task SendVideoAsync(string videoUrl, string recipientId);

    // ⚠️ FOR MESSAGING OUTSIDE 24-HOUR WINDOW
    // Requires specific message tags
    Task SendWithTagAsync(IFacebookMessage message, string recipientId, MessageTagType tag);
    Task SendWithTagAsync(string message, string recipientId, MessageTagType tag);

    // ⚠️ HUMAN_AGENT TAG (7-day window)
    // Requires HUMAN_AGENT permission from App Review
    Task<FacebookSendResult> SendWithHumanAgentFallbackAsync(
        IFacebookMessage message,
        string recipientId);
}
```

### MessageTagType Enum

```csharp
public enum MessageTagType
{
    /// <summary>
    /// Send reminders for confirmed events
    /// </summary>
    ConfirmedEventUpdate,

    /// <summary>
    /// Send order updates (shipping, delivery)
    /// </summary>
    PostPurchaseUpdate,

    /// <summary>
    /// Send account-related updates
    /// </summary>
    AccountUpdate,

    /// <summary>
    /// ⚠️ Extends window to 7 days
    /// ⚠️ Requires HUMAN_AGENT permission from App Review
    /// </summary>
    HumanAgent
}
```

---

## Page-Scoped User ID (PSID)

### Understanding PSID

```
⚠️ PSID is unique per Page, NOT per App!
```

| Concept | Description |
|---------|-------------|
| PSID | Page-Scoped User ID |
| Scope | Unique per Facebook Page |
| Same user, different pages | Different PSIDs |
| Same user, same page | Same PSID |

### Example

```
User "John" interacts with two pages:
- Page A: PSID = "123456789"
- Page B: PSID = "987654321"

These are DIFFERENT IDs for the SAME person!
```

### Implications

```csharp
// Store PSID with Page ID for correct identification
public class Contact
{
    public string Psid { get; set; }           // User's PSID
    public string PageId { get; set; }         // Page this PSID belongs to
    public string DisplayName { get; set; }
}
```

---

## Error Handling

### Common Error Codes

| Code | Meaning | Solution |
|------|---------|----------|
| 10 | Permission denied | Check App Review status |
| 100 | Invalid parameter | Check request format |
| 190 | Access token expired | Refresh page access token |
| 200 | Permission issue | User may have blocked page |
| 551 | 24-hour window expired | Use Message Tags |
| 2018001 | HUMAN_AGENT not approved | Apply for permission |

### Handling 24-Hour Window Expiration

```csharp
try
{
    await facebookClient.Messaging.SendAsync(message, psid);
}
catch (FacebookApiException ex) when (ex.ErrorCode == 551)
{
    // 24-hour window expired, try with Message Tag
    logger.LogWarning("24-hour window expired for {Psid}, using HUMAN_AGENT tag", psid);

    var result = await facebookClient.Messaging.SendWithHumanAgentFallbackAsync(message, psid);

    if (!result.Success && result.HumanAgentTagPermissionDenied)
    {
        // HUMAN_AGENT permission not approved
        logger.LogError("HUMAN_AGENT tag requires App Review approval");
    }
}
```

---

## Rate Limits

| API Type | Limit | Notes |
|----------|-------|-------|
| Send Message | 250/user/second | Per user limit |
| Batch API | 50 calls/batch | Batch request limit |
| Overall | Varies | Based on page quality |

---

## Official Documentation

- [Messenger Platform](https://developers.facebook.com/docs/messenger-platform)
- [Send API](https://developers.facebook.com/docs/messenger-platform/send-messages)
- [Message Tags](https://developers.facebook.com/docs/messenger-platform/send-messages/message-tags)
- [App Review](https://developers.facebook.com/docs/app-review)
- [24-Hour Policy](https://developers.facebook.com/docs/messenger-platform/policy/policy-overview)

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.1.0 | 2025-12-23 | Added API requirements documentation |
| 1.0.0 | 2025-12-01 | Initial release |
