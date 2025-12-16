namespace FacebookSDK.Messages;

/// <summary>
/// Facebook Message Tags - ใช้สำหรับส่งข้อความหลัง 24 ชั่วโมง
/// </summary>
/// <remarks>
/// Message Tags อนุญาตให้ส่งข้อความหลังจากหมดเวลา 24 ชั่วโมง
/// แต่ต้องใช้อย่างถูกต้องตามนโยบายของ Facebook
/// </remarks>
public static class MessageTag
{
    /// <summary>
    /// Confirmed Event Update - สำหรับแจ้งเตือนเกี่ยวกับ event ที่ผู้ใช้ลงทะเบียน
    /// </summary>
    /// <remarks>
    /// ใช้สำหรับ: การเปลี่ยนแปลงเวลา, สถานที่, หรือยกเลิก event
    /// ห้ามใช้สำหรับ: โปรโมชั่น, ข้อความทั่วไป
    /// </remarks>
    public const string ConfirmedEventUpdate = "CONFIRMED_EVENT_UPDATE";

    /// <summary>
    /// Post-Purchase Update - สำหรับแจ้งสถานะการสั่งซื้อ
    /// </summary>
    /// <remarks>
    /// ใช้สำหรับ: ยืนยันการสั่งซื้อ, อัพเดทการจัดส่ง, การเปลี่ยนแปลงคำสั่งซื้อ
    /// ห้ามใช้สำหรับ: โปรโมชั่น, cross-sell, up-sell
    /// </remarks>
    public const string PostPurchaseUpdate = "POST_PURCHASE_UPDATE";

    /// <summary>
    /// Account Update - สำหรับแจ้งเตือนเกี่ยวกับบัญชีผู้ใช้
    /// </summary>
    /// <remarks>
    /// ใช้สำหรับ: การเปลี่ยนแปลง application status, แจ้งเตือนทางบัญชี
    /// ห้ามใช้สำหรับ: เนื้อหาโปรโมชั่น
    /// </remarks>
    public const string AccountUpdate = "ACCOUNT_UPDATE";

    /// <summary>
    /// Human Agent - สำหรับ human agent ตอบกลับภายใน 7 วัน
    /// </summary>
    /// <remarks>
    /// ใช้สำหรับ: Human agent ตอบกลับคำถามของผู้ใช้
    /// ต้องเป็นการตอบจาก human จริง ไม่ใช่ automated message
    /// มีเวลา 7 วันหลังจากผู้ใช้ส่งข้อความมา
    /// </remarks>
    public const string HumanAgent = "HUMAN_AGENT";
}

/// <summary>
/// Message Tag Enum สำหรับ type-safe usage
/// </summary>
public enum MessageTagType
{
    /// <summary>
    /// Confirmed Event Update
    /// </summary>
    ConfirmedEventUpdate,

    /// <summary>
    /// Post-Purchase Update
    /// </summary>
    PostPurchaseUpdate,

    /// <summary>
    /// Account Update
    /// </summary>
    AccountUpdate,

    /// <summary>
    /// Human Agent (7 วัน)
    /// </summary>
    HumanAgent
}

/// <summary>
/// Extension methods for MessageTagType
/// </summary>
public static class MessageTagTypeExtensions
{
    /// <summary>
    /// แปลง MessageTagType เป็น string สำหรับ API
    /// </summary>
    public static string ToApiString(this MessageTagType tag) => tag switch
    {
        MessageTagType.ConfirmedEventUpdate => MessageTag.ConfirmedEventUpdate,
        MessageTagType.PostPurchaseUpdate => MessageTag.PostPurchaseUpdate,
        MessageTagType.AccountUpdate => MessageTag.AccountUpdate,
        MessageTagType.HumanAgent => MessageTag.HumanAgent,
        _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, null)
    };
}
