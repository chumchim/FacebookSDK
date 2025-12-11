using FacebookSDK.Messages;

namespace FacebookSDK.Messaging;

/// <summary>
/// Facebook Messaging Service - ส่งข้อความพื้นฐาน
/// </summary>
public interface IFacebookMessaging
{
    #region Send Message

    /// <summary>
    /// ส่ง text message
    /// </summary>
    Task SendAsync(string message, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่ง Facebook message
    /// </summary>
    Task SendAsync(IFacebookMessage message, string recipientId, CancellationToken ct = default);

    #endregion

    #region Sender Actions

    /// <summary>
    /// แสดงว่ากำลังพิมพ์ (typing indicator)
    /// </summary>
    Task SendTypingOnAsync(string recipientId, CancellationToken ct = default);

    /// <summary>
    /// หยุดแสดงว่ากำลังพิมพ์
    /// </summary>
    Task SendTypingOffAsync(string recipientId, CancellationToken ct = default);

    /// <summary>
    /// แสดงว่าเห็นข้อความแล้ว (mark seen)
    /// </summary>
    Task SendMarkSeenAsync(string recipientId, CancellationToken ct = default);

    #endregion

    #region Message Tags

    /// <summary>
    /// ส่งข้อความพร้อม tag (สำหรับส่งหลัง 24 ชม.)
    /// </summary>
    Task SendWithTagAsync(IFacebookMessage message, string recipientId, string tag, CancellationToken ct = default);

    #endregion
}
