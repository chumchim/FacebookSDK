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

    #region Attachment Messages

    /// <summary>
    /// ส่งรูปภาพ
    /// </summary>
    Task SendImageAsync(string imageUrl, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่งวิดีโอ
    /// </summary>
    Task SendVideoAsync(string videoUrl, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่งไฟล์เสียง
    /// </summary>
    Task SendAudioAsync(string audioUrl, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่งไฟล์
    /// </summary>
    Task SendFileAsync(string fileUrl, string recipientId, CancellationToken ct = default);

    #endregion

    #region Message Tags

    /// <summary>
    /// ส่งข้อความพร้อม tag (สำหรับส่งหลัง 24 ชม.)
    /// </summary>
    Task SendWithTagAsync(IFacebookMessage message, string recipientId, string tag, CancellationToken ct = default);

    /// <summary>
    /// ส่งข้อความพร้อม tag enum (สำหรับส่งหลัง 24 ชม.)
    /// </summary>
    Task SendWithTagAsync(IFacebookMessage message, string recipientId, MessageTagType tag, CancellationToken ct = default);

    /// <summary>
    /// ส่ง text message พร้อม tag (สำหรับส่งหลัง 24 ชม.)
    /// </summary>
    Task SendWithTagAsync(string message, string recipientId, MessageTagType tag, CancellationToken ct = default);

    #endregion

    #region Persona Messages

    /// <summary>
    /// ส่งข้อความในนาม persona
    /// </summary>
    Task SendWithPersonaAsync(IFacebookMessage message, string recipientId, string personaId, CancellationToken ct = default);

    /// <summary>
    /// ส่ง text message ในนาม persona
    /// </summary>
    Task SendWithPersonaAsync(string message, string recipientId, string personaId, CancellationToken ct = default);

    #endregion
}
