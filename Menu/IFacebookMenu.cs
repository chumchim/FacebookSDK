namespace FacebookSDK.Menu;

/// <summary>
/// Facebook Persistent Menu API - จัดการเมนูถาวร
/// </summary>
/// <remarks>
/// Persistent Menu คือเมนูที่แสดงตลอดเวลาใน Messenger
/// ช่วยให้ผู้ใช้เข้าถึงฟีเจอร์หลักได้ง่าย
///
/// Usage:
/// <code>
/// var menu = new PersistentMenu
/// {
///     CallToActions = new List&lt;MenuItem&gt;
///     {
///         MenuItem.CreatePostback("ช่วยเหลือ", "HELP"),
///         MenuItem.CreateWebUrl("เว็บไซต์", "https://example.com"),
///         MenuItem.CreateNested("อื่นๆ", new List&lt;MenuItem&gt;
///         {
///             MenuItem.CreatePostback("ติดต่อเรา", "CONTACT")
///         })
///     }
/// };
/// await facebook.Menu.SetAsync(menu);
/// </code>
/// </remarks>
public interface IFacebookMenu
{
    /// <summary>
    /// ตั้งค่า persistent menu
    /// </summary>
    /// <param name="menu">Menu configuration</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> SetAsync(PersistentMenu menu, CancellationToken ct = default);

    /// <summary>
    /// ตั้งค่า persistent menu สำหรับ locale เฉพาะ
    /// </summary>
    /// <param name="menus">Menu configurations per locale</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> SetAsync(List<PersistentMenu> menus, CancellationToken ct = default);

    /// <summary>
    /// ดึงการตั้งค่า persistent menu ปัจจุบัน
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    Task<List<PersistentMenu>> GetAsync(CancellationToken ct = default);

    /// <summary>
    /// ลบ persistent menu
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    Task<bool> DeleteAsync(CancellationToken ct = default);
}

/// <summary>
/// Persistent Menu Configuration
/// </summary>
public class PersistentMenu
{
    /// <summary>
    /// Locale (default: "default" สำหรับทุกภาษา)
    /// </summary>
    public string Locale { get; set; } = "default";

    /// <summary>
    /// เปิด/ปิดการใช้งาน composer input
    /// </summary>
    public bool ComposerInputDisabled { get; set; } = false;

    /// <summary>
    /// รายการ menu items (สูงสุด 3 items)
    /// </summary>
    public List<MenuItem> CallToActions { get; set; } = new();
}

/// <summary>
/// Menu Item
/// </summary>
public class MenuItem
{
    /// <summary>
    /// ประเภท menu item
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// ข้อความที่แสดง
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Payload สำหรับ postback (ใช้กับ type = postback)
    /// </summary>
    public string? Payload { get; set; }

    /// <summary>
    /// URL สำหรับ web_url (ใช้กับ type = web_url)
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Webview height ratio
    /// </summary>
    public string? WebviewHeightRatio { get; set; }

    /// <summary>
    /// เปิด Messenger Extensions
    /// </summary>
    public bool? MessengerExtensions { get; set; }

    /// <summary>
    /// Fallback URL
    /// </summary>
    public string? FallbackUrl { get; set; }

    /// <summary>
    /// Webview share button (ซ่อนหรือไม่)
    /// </summary>
    public string? WebviewShareButton { get; set; }

    /// <summary>
    /// Nested menu items (ใช้กับ type = nested)
    /// </summary>
    public List<MenuItem>? CallToActions { get; set; }

    /// <summary>
    /// สร้าง Postback menu item
    /// </summary>
    public static MenuItem CreatePostback(string title, string payload) => new()
    {
        Type = "postback",
        Title = title,
        Payload = payload
    };

    /// <summary>
    /// สร้าง Web URL menu item
    /// </summary>
    public static MenuItem CreateWebUrl(string title, string url, WebviewHeightRatioType heightRatio = WebviewHeightRatioType.Full) => new()
    {
        Type = "web_url",
        Title = title,
        Url = url,
        WebviewHeightRatio = heightRatio.ToApiString()
    };

    /// <summary>
    /// สร้าง Web URL menu item พร้อม Messenger Extensions
    /// </summary>
    public static MenuItem CreateWebUrlWithExtensions(string title, string url, string? fallbackUrl = null, WebviewHeightRatioType heightRatio = WebviewHeightRatioType.Full) => new()
    {
        Type = "web_url",
        Title = title,
        Url = url,
        WebviewHeightRatio = heightRatio.ToApiString(),
        MessengerExtensions = true,
        FallbackUrl = fallbackUrl ?? url
    };

    /// <summary>
    /// สร้าง Nested menu item
    /// </summary>
    public static MenuItem CreateNested(string title, List<MenuItem> items) => new()
    {
        Type = "nested",
        Title = title,
        CallToActions = items
    };
}

/// <summary>
/// Webview Height Ratio
/// </summary>
public enum WebviewHeightRatioType
{
    /// <summary>
    /// Compact (50% height)
    /// </summary>
    Compact,

    /// <summary>
    /// Tall (75% height)
    /// </summary>
    Tall,

    /// <summary>
    /// Full (100% height)
    /// </summary>
    Full
}

/// <summary>
/// Extension methods for WebviewHeightRatioType
/// </summary>
public static class WebviewHeightRatioExtensions
{
    public static string ToApiString(this WebviewHeightRatioType ratio) => ratio switch
    {
        WebviewHeightRatioType.Compact => "compact",
        WebviewHeightRatioType.Tall => "tall",
        WebviewHeightRatioType.Full => "full",
        _ => "full"
    };
}
