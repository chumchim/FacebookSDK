namespace FacebookSDK.Conversations;

/// <summary>
/// Facebook Conversations API - ดึงประวัติการสนทนา
/// </summary>
public interface IFacebookConversation
{
    /// <summary>
    /// ดึงรายการ conversations ทั้งหมด
    /// </summary>
    /// <param name="limit">จำนวน conversations (default: 20)</param>
    /// <param name="ct">Cancellation token</param>
    Task<ConversationListResponse> GetConversationsAsync(int limit = 20, CancellationToken ct = default);

    /// <summary>
    /// ดึงรายการ conversations พร้อม pagination
    /// </summary>
    /// <param name="afterCursor">Cursor สำหรับหน้าถัดไป</param>
    /// <param name="limit">จำนวน conversations</param>
    /// <param name="ct">Cancellation token</param>
    Task<ConversationListResponse> GetConversationsAsync(string afterCursor, int limit = 20, CancellationToken ct = default);

    /// <summary>
    /// ดึง messages ใน conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="limit">จำนวน messages (default: 20)</param>
    /// <param name="ct">Cancellation token</param>
    Task<MessageListResponse> GetMessagesAsync(string conversationId, int limit = 20, CancellationToken ct = default);

    /// <summary>
    /// ดึง messages พร้อม pagination
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="afterCursor">Cursor สำหรับหน้าถัดไป</param>
    /// <param name="limit">จำนวน messages</param>
    /// <param name="ct">Cancellation token</param>
    Task<MessageListResponse> GetMessagesAsync(string conversationId, string afterCursor, int limit = 20, CancellationToken ct = default);

    /// <summary>
    /// ดึง message เดียว พร้อมรายละเอียด
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="ct">Cancellation token</param>
    Task<ConversationMessage?> GetMessageAsync(string messageId, CancellationToken ct = default);

    /// <summary>
    /// ดึง conversation โดย PSID
    /// </summary>
    /// <param name="psid">Page-scoped user ID</param>
    /// <param name="ct">Cancellation token</param>
    Task<Conversation?> GetConversationByPsidAsync(string psid, CancellationToken ct = default);
}

#region Response Models

/// <summary>
/// Conversation List Response
/// </summary>
public class ConversationListResponse
{
    public List<Conversation> Data { get; set; } = new();
    public Paging? Paging { get; set; }
}

/// <summary>
/// Conversation
/// </summary>
public class Conversation
{
    public string Id { get; set; } = string.Empty;
    public string? Link { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public List<ConversationParticipant>? Participants { get; set; }
    public int? UnreadCount { get; set; }
}

/// <summary>
/// Conversation Participant
/// </summary>
public class ConversationParticipant
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// Message List Response
/// </summary>
public class MessageListResponse
{
    public List<ConversationMessage> Data { get; set; } = new();
    public Paging? Paging { get; set; }
}

/// <summary>
/// Conversation Message
/// </summary>
public class ConversationMessage
{
    public string Id { get; set; } = string.Empty;
    public string? Message { get; set; }
    public MessageFrom? From { get; set; }
    public List<MessageTo>? To { get; set; }
    public DateTime? CreatedTime { get; set; }
    public List<MessageAttachment>? Attachments { get; set; }
    public MessageSticker? Sticker { get; set; }
    public List<MessageShare>? Shares { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Message From/To
/// </summary>
public class MessageFrom
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public class MessageTo
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// Message Attachment
/// </summary>
public class MessageAttachment
{
    public string Id { get; set; } = string.Empty;
    public string? MimeType { get; set; }
    public string? Name { get; set; }
    public int? Size { get; set; }
    public ImageData? ImageData { get; set; }
    public VideoData? VideoData { get; set; }
    public string? FileUrl { get; set; }
}

public class ImageData
{
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? MaxWidth { get; set; }
    public int? MaxHeight { get; set; }
    public string? Url { get; set; }
    public string? PreviewUrl { get; set; }
}

public class VideoData
{
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Length { get; set; }
    public string? Url { get; set; }
    public string? PreviewUrl { get; set; }
}

/// <summary>
/// Message Sticker
/// </summary>
public class MessageSticker
{
    public string Id { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? PackId { get; set; }
}

/// <summary>
/// Message Share
/// </summary>
public class MessageShare
{
    public string? Description { get; set; }
    public string? Link { get; set; }
    public string? Name { get; set; }
}

/// <summary>
/// Pagination
/// </summary>
public class Paging
{
    public Cursors? Cursors { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
}

public class Cursors
{
    public string? Before { get; set; }
    public string? After { get; set; }
}

#endregion
