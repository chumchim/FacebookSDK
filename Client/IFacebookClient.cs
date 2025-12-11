using FacebookSDK.Messaging;
using FacebookSDK.Profile;

namespace FacebookSDK.Client;

/// <summary>
/// Facebook Client - Entry point สำหรับใช้งาน Facebook SDK
/// </summary>
/// <remarks>
/// ใช้งาน:
/// <code>
/// // DI
/// services.AddFacebookClient(options => {
///     options.PageAccessToken = "...";
///     options.AppSecret = "...";
///     options.VerifyToken = "...";
/// });
///
/// // Inject
/// public class MyService(IFacebookClient facebook)
/// {
///     await facebook.Messaging.SendAsync("Hello", recipientId);
///     var profile = await facebook.Profile.GetUserProfileAsync(userId);
/// }
/// </code>
/// </remarks>
public interface IFacebookClient
{
    /// <summary>
    /// Messaging - ส่งข้อความ
    /// </summary>
    IFacebookMessaging Messaging { get; }

    /// <summary>
    /// Profile - จัดการข้อมูล profile
    /// </summary>
    IFacebookProfile Profile { get; }
}
