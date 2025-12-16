namespace FacebookSDK.Handover;

/// <summary>
/// Facebook Handover Protocol API - ส่งต่อการสนทนาระหว่าง bot กับ human agent
/// </summary>
/// <remarks>
/// Handover Protocol ใช้สำหรับ:
/// - ส่งต่อการสนทนาจาก bot ไปยัง human agent
/// - ส่งกลับจาก human agent ไปยัง bot
/// - จัดการ thread control ระหว่าง apps
///
/// Usage:
/// <code>
/// // Pass conversation to human agent (Page Inbox)
/// await facebook.Handover.PassThreadControlAsync(recipientId, HandoverTarget.PageInbox);
///
/// // Take back control to bot
/// await facebook.Handover.TakeThreadControlAsync(recipientId);
/// </code>
/// </remarks>
public interface IFacebookHandover
{
    /// <summary>
    /// ส่งต่อ thread control ไปยัง app อื่น
    /// </summary>
    /// <param name="recipientId">PSID ของผู้ใช้</param>
    /// <param name="targetAppId">App ID ที่ต้องการส่งต่อ</param>
    /// <param name="metadata">Metadata เพิ่มเติม (optional)</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> PassThreadControlAsync(string recipientId, string targetAppId, string? metadata = null, CancellationToken ct = default);

    /// <summary>
    /// ส่งต่อ thread control ไปยัง target ที่กำหนด
    /// </summary>
    /// <param name="recipientId">PSID ของผู้ใช้</param>
    /// <param name="target">Target ที่ต้องการส่งต่อ</param>
    /// <param name="metadata">Metadata เพิ่มเติม (optional)</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> PassThreadControlAsync(string recipientId, HandoverTarget target, string? metadata = null, CancellationToken ct = default);

    /// <summary>
    /// เอา thread control กลับมา (ใช้โดย primary receiver)
    /// </summary>
    /// <param name="recipientId">PSID ของผู้ใช้</param>
    /// <param name="metadata">Metadata เพิ่มเติม (optional)</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> TakeThreadControlAsync(string recipientId, string? metadata = null, CancellationToken ct = default);

    /// <summary>
    /// ขอ thread control จาก primary receiver
    /// </summary>
    /// <param name="recipientId">PSID ของผู้ใช้</param>
    /// <param name="metadata">Metadata เพิ่มเติม (optional)</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> RequestThreadControlAsync(string recipientId, string? metadata = null, CancellationToken ct = default);

    /// <summary>
    /// ดึงข้อมูล thread owner ปัจจุบัน
    /// </summary>
    /// <param name="recipientId">PSID ของผู้ใช้</param>
    /// <param name="ct">Cancellation token</param>
    Task<ThreadOwnerInfo?> GetThreadOwnerAsync(string recipientId, CancellationToken ct = default);

    /// <summary>
    /// ดึงรายการ secondary receivers
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    Task<List<SecondaryReceiverInfo>> GetSecondaryReceiversAsync(CancellationToken ct = default);
}

/// <summary>
/// Handover Target - เป้าหมายที่จะส่งต่อการสนทนา
/// </summary>
public enum HandoverTarget
{
    /// <summary>
    /// Page Inbox - ส่งต่อไปยัง Facebook Page Inbox (human agent)
    /// </summary>
    PageInbox,

    /// <summary>
    /// Primary Receiver - ส่งกลับไปยัง primary receiver (bot)
    /// </summary>
    PrimaryReceiver
}

/// <summary>
/// Well-known App IDs for Handover
/// </summary>
public static class HandoverAppIds
{
    /// <summary>
    /// Page Inbox App ID
    /// </summary>
    public const string PageInbox = "263902037430900";
}

/// <summary>
/// Thread Owner Information
/// </summary>
public class ThreadOwnerInfo
{
    /// <summary>
    /// App ID ที่เป็นเจ้าของ thread
    /// </summary>
    public string? AppId { get; set; }
}

/// <summary>
/// Secondary Receiver Information
/// </summary>
public class SecondaryReceiverInfo
{
    /// <summary>
    /// App ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// App Name
    /// </summary>
    public string? Name { get; set; }
}
