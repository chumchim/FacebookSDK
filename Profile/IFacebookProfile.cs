using FacebookSDK.Models;

namespace FacebookSDK.Profile;

/// <summary>
/// Facebook Profile Service - จัดการข้อมูล profile
/// </summary>
public interface IFacebookProfile
{
    /// <summary>
    /// ดึง profile ของ user
    /// </summary>
    Task<FacebookUserProfile?> GetUserProfileAsync(string userId, CancellationToken ct = default);

    /// <summary>
    /// ดึง profile ของ user พร้อมระบุ fields ที่ต้องการ
    /// </summary>
    Task<FacebookUserProfile?> GetUserProfileAsync(string userId, string[] fields, CancellationToken ct = default);

    /// <summary>
    /// ดึง profile ของ user ผ่าน Conversations API (ไม่ต้อง App Review)
    /// </summary>
    Task<FacebookUserProfile?> GetUserProfileViaConversationsAsync(string psid, CancellationToken ct = default);
}
