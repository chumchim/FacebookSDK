using FacebookSDK.Conversations;
using FacebookSDK.Handover;
using FacebookSDK.IceBreakers;
using FacebookSDK.Menu;
using FacebookSDK.Messaging;
using FacebookSDK.Notifications;
using FacebookSDK.Persona;
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
///     // Send message
///     await facebook.Messaging.SendAsync("Hello", recipientId);
///
///     // Get profile
///     var profile = await facebook.Profile.GetUserProfileAsync(userId);
///
///     // Get conversation history
///     var messages = await facebook.Conversation.GetMessagesAsync(conversationId);
///
///     // Pass to human agent
///     await facebook.Handover.PassThreadControlAsync(recipientId, HandoverTarget.PageInbox);
///
///     // Create persona
///     var persona = await facebook.Persona.CreateAsync("Agent", "https://example.com/agent.jpg");
///
///     // Set persistent menu
///     await facebook.Menu.SetAsync(new PersistentMenu { ... });
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

    /// <summary>
    /// Conversation - ดึงประวัติการสนทนา
    /// </summary>
    IFacebookConversation Conversation { get; }

    /// <summary>
    /// Handover - ส่งต่อการสนทนาระหว่าง bot กับ human agent
    /// </summary>
    IFacebookHandover Handover { get; }

    /// <summary>
    /// Persona - สร้างตัวตนสำหรับส่งข้อความ
    /// </summary>
    IFacebookPersona Persona { get; }

    /// <summary>
    /// Menu - จัดการ Persistent Menu
    /// </summary>
    IFacebookMenu Menu { get; }

    /// <summary>
    /// IceBreakers - คำถามเริ่มต้นสำหรับผู้ใช้ใหม่
    /// </summary>
    IFacebookIceBreakers IceBreakers { get; }

    /// <summary>
    /// Notification - One-Time Notification
    /// </summary>
    IFacebookNotification Notification { get; }
}

