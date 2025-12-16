namespace FacebookSDK.IceBreakers;

/// <summary>
/// Facebook Ice Breakers API - คำถามเริ่มต้นสำหรับผู้ใช้ใหม่
/// </summary>
/// <remarks>
/// Ice Breakers คือคำถามที่แสดงให้ผู้ใช้ใหม่เห็นเมื่อเริ่มต้นสนทนา
/// ช่วยให้ผู้ใช้รู้ว่าสามารถถามอะไรได้บ้าง
///
/// Usage:
/// <code>
/// var iceBreakers = new List&lt;IceBreaker&gt;
/// {
///     new IceBreaker { Question = "สินค้าของคุณมีอะไรบ้าง?", Payload = "GET_PRODUCTS" },
///     new IceBreaker { Question = "ติดต่อฝ่ายบริการลูกค้า", Payload = "CONTACT_SUPPORT" },
///     new IceBreaker { Question = "ดูโปรโมชั่น", Payload = "GET_PROMOTIONS" }
/// };
/// await facebook.IceBreakers.SetAsync(iceBreakers);
/// </code>
/// </remarks>
public interface IFacebookIceBreakers
{
    /// <summary>
    /// ตั้งค่า ice breakers
    /// </summary>
    /// <param name="iceBreakers">รายการ ice breakers (สูงสุด 4)</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> SetAsync(List<IceBreaker> iceBreakers, CancellationToken ct = default);

    /// <summary>
    /// ดึงการตั้งค่า ice breakers ปัจจุบัน
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    Task<List<IceBreaker>> GetAsync(CancellationToken ct = default);

    /// <summary>
    /// ลบ ice breakers
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    Task<bool> DeleteAsync(CancellationToken ct = default);
}

/// <summary>
/// Ice Breaker - คำถามเริ่มต้น
/// </summary>
public class IceBreaker
{
    /// <summary>
    /// คำถาม/ข้อความที่แสดง (สูงสุด 80 ตัวอักษร)
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Payload ที่จะส่งเมื่อผู้ใช้เลือก
    /// </summary>
    public string Payload { get; set; } = string.Empty;
}
