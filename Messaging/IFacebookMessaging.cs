using FacebookSDK.Messages;

namespace FacebookSDK.Messaging;

/// <summary>
/// Facebook Messaging Service - ส่งข้อความพื้นฐาน
/// </summary>
/// <remarks>
/// <para><b>IMPORTANT: App Review Required!</b></para>
/// <para>Most Messenger features require App Review approval with:</para>
/// <list type="bullet">
///   <item>Video walkthrough showing feature usage</item>
///   <item>Privacy policy URL</item>
///   <item>Detailed description of use case</item>
/// </list>
/// <para><b>24-Hour Messaging Window:</b></para>
/// <para>Standard messaging only works within 24 hours of user's last message.</para>
/// <para>After 24 hours, you must use Message Tags.</para>
/// <para>See README.md for full requirements.</para>
/// </remarks>
public interface IFacebookMessaging
{
    #region Send Message

    /// <summary>
    /// ส่ง text message
    /// </summary>
    /// <remarks>
    /// <para><b>24-Hour Window:</b> Only works within 24 hours of user's last message.</para>
    /// <para><b>Error 551:</b> Thrown if 24-hour window has expired.</para>
    /// <para>Use SendWithTagAsync() or SendWithHumanAgentFallbackAsync() for messages outside window.</para>
    /// </remarks>
    /// <param name="message">Text message content</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="ct">Cancellation token</param>
    Task SendAsync(string message, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่ง Facebook message
    /// </summary>
    /// <remarks>
    /// <para><b>24-Hour Window:</b> Only works within 24 hours of user's last message.</para>
    /// <para><b>Error 551:</b> Thrown if 24-hour window has expired.</para>
    /// <para>Use SendWithTagAsync() or SendWithHumanAgentFallbackAsync() for messages outside window.</para>
    /// </remarks>
    /// <param name="message">Facebook message object</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="ct">Cancellation token</param>
    Task SendAsync(IFacebookMessage message, string recipientId, CancellationToken ct = default);

    #endregion

    #region Sender Actions

    /// <summary>
    /// แสดงว่ากำลังพิมพ์ (typing indicator)
    /// </summary>
    /// <remarks>
    /// <para><b>Note:</b> Automatically disappears after 20 seconds.</para>
    /// </remarks>
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

    #region Attachment Messages (URL-based)

    /// <summary>
    /// ส่งรูปภาพ (ใช้ URL - Facebook จะมา fetch)
    /// </summary>
    /// <remarks>
    /// <para><b>24-Hour Window:</b> Only works within 24 hours of user's last message.</para>
    /// <para><b>Limits:</b> Max 25MB, supported formats: jpg, png, gif</para>
    /// </remarks>
    Task SendImageAsync(string imageUrl, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่งวิดีโอ (ใช้ URL - Facebook จะมา fetch)
    /// </summary>
    /// <remarks>
    /// <para><b>24-Hour Window:</b> Only works within 24 hours of user's last message.</para>
    /// <para><b>Limits:</b> Max 25MB</para>
    /// </remarks>
    Task SendVideoAsync(string videoUrl, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่งไฟล์เสียง (ใช้ URL - Facebook จะมา fetch)
    /// </summary>
    /// <remarks>
    /// <para><b>24-Hour Window:</b> Only works within 24 hours of user's last message.</para>
    /// <para><b>Limits:</b> Max 25MB</para>
    /// </remarks>
    Task SendAudioAsync(string audioUrl, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่งไฟล์ (ใช้ URL - Facebook จะมา fetch)
    /// </summary>
    /// <remarks>
    /// <para><b>24-Hour Window:</b> Only works within 24 hours of user's last message.</para>
    /// <para><b>Limits:</b> Max 25MB</para>
    /// </remarks>
    Task SendFileAsync(string fileUrl, string recipientId, CancellationToken ct = default);

    #endregion

    #region Attachment Upload (Binary upload)

    /// <summary>
    /// Upload และส่งรูปภาพโดยตรง (ไม่ต้องมี public URL)
    /// </summary>
    Task<FacebookSendResult> UploadAndSendImageAsync(
        byte[] imageData,
        string fileName,
        string recipientId,
        CancellationToken ct = default);

    /// <summary>
    /// Upload และส่งรูปภาพโดยตรง (ไม่ต้องมี public URL) พร้อม HUMAN_AGENT fallback
    /// </summary>
    Task<FacebookSendResult> UploadAndSendImageWithFallbackAsync(
        byte[] imageData,
        string fileName,
        string recipientId,
        CancellationToken ct = default);

    /// <summary>
    /// Upload และส่งไฟล์โดยตรง (ไม่ต้องมี public URL)
    /// </summary>
    Task<FacebookSendResult> UploadAndSendFileAsync(
        byte[] fileData,
        string fileName,
        string contentType,
        string recipientId,
        CancellationToken ct = default);

    #endregion

    #region Message Tags

    /// <summary>
    /// ส่งข้อความพร้อม tag (สำหรับส่งหลัง 24 ชม.)
    /// </summary>
    /// <remarks>
    /// <para><b>Purpose:</b> Send messages outside 24-hour window.</para>
    /// <para><b>Available Tags:</b></para>
    /// <list type="bullet">
    ///   <item>CONFIRMED_EVENT_UPDATE - Event reminders</item>
    ///   <item>POST_PURCHASE_UPDATE - Order/shipping updates</item>
    ///   <item>ACCOUNT_UPDATE - Account changes</item>
    ///   <item>HUMAN_AGENT - Live agent support (7-day window, requires approval)</item>
    /// </list>
    /// <para>See: https://developers.facebook.com/docs/messenger-platform/send-messages/message-tags</para>
    /// </remarks>
    /// <param name="message">Facebook message object</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="tag">Message tag string</param>
    /// <param name="ct">Cancellation token</param>
    Task SendWithTagAsync(IFacebookMessage message, string recipientId, string tag, CancellationToken ct = default);

    /// <summary>
    /// ส่งข้อความพร้อม tag enum (สำหรับส่งหลัง 24 ชม.)
    /// </summary>
    /// <remarks>
    /// <para><b>Purpose:</b> Send messages outside 24-hour window.</para>
    /// <para><b>HUMAN_AGENT Tag:</b> Extends window to 7 days but requires App Review approval.</para>
    /// <para><b>Error 2018001:</b> HUMAN_AGENT permission not approved.</para>
    /// </remarks>
    /// <param name="message">Facebook message object</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="tag">Message tag type</param>
    /// <param name="ct">Cancellation token</param>
    Task SendWithTagAsync(IFacebookMessage message, string recipientId, MessageTagType tag, CancellationToken ct = default);

    /// <summary>
    /// ส่ง text message พร้อม tag (สำหรับส่งหลัง 24 ชม.)
    /// </summary>
    /// <remarks>
    /// <para><b>Purpose:</b> Send messages outside 24-hour window.</para>
    /// <para><b>HUMAN_AGENT Tag:</b> Extends window to 7 days but requires App Review approval.</para>
    /// </remarks>
    /// <param name="message">Text message content</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="tag">Message tag type</param>
    /// <param name="ct">Cancellation token</param>
    Task SendWithTagAsync(string message, string recipientId, MessageTagType tag, CancellationToken ct = default);

    #endregion

    #region Persona Messages

    /// <summary>
    /// ส่งข้อความในนาม persona
    /// </summary>
    /// <remarks>
    /// <para><b>24-Hour Window:</b> Only works within 24 hours of user's last message.</para>
    /// <para><b>Persona:</b> Custom sender name and profile picture.</para>
    /// </remarks>
    Task SendWithPersonaAsync(IFacebookMessage message, string recipientId, string personaId, CancellationToken ct = default);

    /// <summary>
    /// ส่ง text message ในนาม persona
    /// </summary>
    /// <remarks>
    /// <para><b>24-Hour Window:</b> Only works within 24 hours of user's last message.</para>
    /// </remarks>
    Task SendWithPersonaAsync(string message, string recipientId, string personaId, CancellationToken ct = default);

    #endregion

    #region Fallback Methods (Human Agent Tag)

    /// <summary>
    /// ส่งข้อความด้วย fallback strategy:
    /// 1. ลองส่งแบบปกติก่อน (ภายใน 24 ชม.)
    /// 2. ถ้าล้มเหลว ลองส่งด้วย HUMAN_AGENT tag (7 วัน)
    /// 3. ถ้า HUMAN_AGENT tag ไม่ได้รับอนุญาต จะ return error พร้อม reason
    /// </summary>
    /// <remarks>
    /// <para><b>Recommended for Call Center:</b> Automatically handles 24-hour window expiration.</para>
    /// <para><b>HUMAN_AGENT Permission:</b> Requires App Review approval.</para>
    /// <para><b>If HUMAN_AGENT denied:</b> Result.HumanAgentTagPermissionDenied = true</para>
    /// <para>Apply for HUMAN_AGENT permission at: Facebook Developer Console > App Review</para>
    /// </remarks>
    /// <param name="message">Facebook message object</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Result with success status and method used</returns>
    Task<FacebookSendResult> SendWithHumanAgentFallbackAsync(IFacebookMessage message, string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ส่ง text message ด้วย fallback strategy
    /// </summary>
    /// <remarks>
    /// <para><b>Recommended for Call Center:</b> Automatically handles 24-hour window expiration.</para>
    /// <para><b>HUMAN_AGENT Permission:</b> Requires App Review approval.</para>
    /// </remarks>
    /// <param name="message">Text message content</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Result with success status and method used</returns>
    Task<FacebookSendResult> SendWithHumanAgentFallbackAsync(string message, string recipientId, CancellationToken ct = default);

    #endregion

    #region Upload and Send (File Data)

    /// <summary>
    /// อัปโหลดรูปภาพและส่งพร้อม fallback strategy
    /// </summary>
    /// <remarks>
    /// <para><b>Upload + Send:</b> Uploads image data to Facebook, then sends to recipient.</para>
    /// <para><b>Fallback:</b> Tries standard messaging first, then HUMAN_AGENT tag if outside 24h window.</para>
    /// <para><b>Limits:</b> Max 25MB</para>
    /// </remarks>
    /// <param name="fileData">Image file data as byte array</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Result with success status and method used</returns>
    Task<FacebookSendResult> UploadAndSendImageWithFallbackAsync(
        byte[] fileData,
        string fileName,
        string recipientId,
        CancellationToken ct = default);

    /// <summary>
    /// อัปโหลดไฟล์และส่งพร้อม fallback strategy
    /// </summary>
    /// <remarks>
    /// <para><b>Upload + Send:</b> Uploads file data to Facebook, then sends to recipient.</para>
    /// <para><b>Fallback:</b> Tries standard messaging first, then HUMAN_AGENT tag if outside 24h window.</para>
    /// <para><b>Limits:</b> Max 25MB</para>
    /// </remarks>
    /// <param name="fileData">File data as byte array</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="recipientId">PSID (Page-Scoped User ID)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Result with success status and method used</returns>
    Task<FacebookSendResult> UploadAndSendFileAsync(
        byte[] fileData,
        string fileName,
        string contentType,
        string recipientId,
        CancellationToken ct = default);

    #endregion
}

/// <summary>
/// Result of Facebook send operation with fallback
/// </summary>
public class FacebookSendResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public FacebookSendMethod MethodUsed { get; init; }
    public bool HumanAgentTagRequired { get; init; }
    public bool HumanAgentTagPermissionDenied { get; init; }

    public static FacebookSendResult Ok(FacebookSendMethod method = FacebookSendMethod.Standard)
        => new() { Success = true, MethodUsed = method };

    public static FacebookSendResult Failed(string error, bool humanAgentRequired = false, bool humanAgentDenied = false)
        => new()
        {
            Success = false,
            ErrorMessage = error,
            HumanAgentTagRequired = humanAgentRequired,
            HumanAgentTagPermissionDenied = humanAgentDenied
        };
}

/// <summary>
/// Method used to send Facebook message
/// </summary>
public enum FacebookSendMethod
{
    /// <summary>Standard messaging (within 24h window)</summary>
    Standard,
    /// <summary>HUMAN_AGENT tag (7 day window)</summary>
    HumanAgentTag
}
