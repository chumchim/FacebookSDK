namespace FacebookSDK.Notifications;

/// <summary>
/// Facebook One-Time Notification API - ขออนุญาตส่งข้อความเดียว
/// </summary>
/// <remarks>
/// One-Time Notification Request ช่วยให้สามารถ:
/// - ขออนุญาตจากผู้ใช้ในการส่งข้อความครั้งเดียวหลัง 24 ชั่วโมง
/// - ส่ง notification ไปยังผู้ใช้ที่อนุญาตแล้ว
///
/// Flow:
/// 1. ส่ง One-Time Notification Request ให้ผู้ใช้
/// 2. ผู้ใช้กด "Notify Me"
/// 3. รับ notification token จาก webhook (messaging_optins event)
/// 4. ใช้ token ส่งข้อความได้ครั้งเดียว
///
/// Usage:
/// <code>
/// // 1. Request permission
/// await facebook.Notification.SendRequestAsync(
///     recipientId,
///     "แจ้งเตือนเมื่อสินค้าพร้อมจัดส่ง",
///     "SHIPPING_UPDATE"
/// );
///
/// // 2. When user accepts (from webhook), you get a token
/// // 3. Send the notification
/// await facebook.Notification.SendAsync(token, "สินค้าของคุณพร้อมจัดส่งแล้ว!");
/// </code>
/// </remarks>
public interface IFacebookNotification
{
    /// <summary>
    /// ส่ง One-Time Notification Request ให้ผู้ใช้
    /// </summary>
    /// <param name="recipientId">PSID ของผู้ใช้</param>
    /// <param name="title">ข้อความที่แสดงให้ผู้ใช้เห็น</param>
    /// <param name="payload">Payload สำหรับ identify request</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> SendRequestAsync(string recipientId, string title, string payload, CancellationToken ct = default);

    /// <summary>
    /// ส่งข้อความโดยใช้ One-Time Notification Token
    /// </summary>
    /// <param name="token">Token ที่ได้จาก messaging_optins webhook</param>
    /// <param name="message">ข้อความที่ต้องการส่ง</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> SendAsync(string token, string message, CancellationToken ct = default);

    /// <summary>
    /// ส่งข้อความโดยใช้ One-Time Notification Token พร้อม template
    /// </summary>
    /// <param name="token">Token ที่ได้จาก messaging_optins webhook</param>
    /// <param name="message">Facebook message object</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> SendAsync(string token, Messages.IFacebookMessage message, CancellationToken ct = default);
}

/// <summary>
/// One-Time Notification Opt-in Event (จาก webhook)
/// </summary>
public class OneTimeNotificationOptIn
{
    /// <summary>
    /// Notification token สำหรับส่งข้อความ
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token expiry timestamp
    /// </summary>
    public long TokenExpiryTimestamp { get; set; }

    /// <summary>
    /// Payload ที่ส่งไปตอน request
    /// </summary>
    public string? Payload { get; set; }

    /// <summary>
    /// User ref (ถ้ามี)
    /// </summary>
    public string? UserRef { get; set; }

    /// <summary>
    /// ตรวจสอบว่า token หมดอายุหรือไม่
    /// </summary>
    public bool IsExpired => DateTimeOffset.UtcNow.ToUnixTimeSeconds() > TokenExpiryTimestamp;

    /// <summary>
    /// วันเวลาหมดอายุ
    /// </summary>
    public DateTimeOffset ExpiryTime => DateTimeOffset.FromUnixTimeSeconds(TokenExpiryTimestamp);
}
