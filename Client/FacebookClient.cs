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
/// Facebook Client Implementation
/// </summary>
public class FacebookClient : IFacebookClient
{
    public FacebookClient(
        IFacebookMessaging messaging,
        IFacebookProfile profile,
        IFacebookConversation conversation,
        IFacebookHandover handover,
        IFacebookPersona persona,
        IFacebookMenu menu,
        IFacebookIceBreakers iceBreakers,
        IFacebookNotification notification)
    {
        Messaging = messaging;
        Profile = profile;
        Conversation = conversation;
        Handover = handover;
        Persona = persona;
        Menu = menu;
        IceBreakers = iceBreakers;
        Notification = notification;
    }

    /// <inheritdoc />
    public IFacebookMessaging Messaging { get; }

    /// <inheritdoc />
    public IFacebookProfile Profile { get; }

    /// <inheritdoc />
    public IFacebookConversation Conversation { get; }

    /// <inheritdoc />
    public IFacebookHandover Handover { get; }

    /// <inheritdoc />
    public IFacebookPersona Persona { get; }

    /// <inheritdoc />
    public IFacebookMenu Menu { get; }

    /// <inheritdoc />
    public IFacebookIceBreakers IceBreakers { get; }

    /// <inheritdoc />
    public IFacebookNotification Notification { get; }
}

