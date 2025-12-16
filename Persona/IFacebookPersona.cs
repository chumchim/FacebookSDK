namespace FacebookSDK.Persona;

/// <summary>
/// Facebook Persona API - สร้างตัวตนสำหรับส่งข้อความ
/// </summary>
/// <remarks>
/// Persona ช่วยให้สามารถส่งข้อความในนามของบุคคลเฉพาะ
/// เช่น ส่งในนาม agent หรือแผนกที่ต้องการ
///
/// Usage:
/// <code>
/// var persona = await facebook.Persona.CreateAsync("John", "https://example.com/john.jpg");
/// await facebook.Messaging.SendWithPersonaAsync("Hello", recipientId, persona.Id);
/// </code>
/// </remarks>
public interface IFacebookPersona
{
    /// <summary>
    /// สร้าง persona ใหม่
    /// </summary>
    /// <param name="name">ชื่อ persona</param>
    /// <param name="profilePictureUrl">URL รูปโปรไฟล์</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Persona ที่สร้างใหม่</returns>
    Task<PersonaInfo> CreateAsync(string name, string profilePictureUrl, CancellationToken ct = default);

    /// <summary>
    /// ดึง persona ตาม ID
    /// </summary>
    /// <param name="personaId">Persona ID</param>
    /// <param name="ct">Cancellation token</param>
    Task<PersonaInfo?> GetAsync(string personaId, CancellationToken ct = default);

    /// <summary>
    /// ดึงรายการ personas ทั้งหมด
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    Task<List<PersonaInfo>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// ลบ persona
    /// </summary>
    /// <param name="personaId">Persona ID</param>
    /// <param name="ct">Cancellation token</param>
    Task<bool> DeleteAsync(string personaId, CancellationToken ct = default);
}

/// <summary>
/// Persona Information
/// </summary>
public class PersonaInfo
{
    /// <summary>
    /// Persona ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ชื่อ persona
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL รูปโปรไฟล์
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
}
