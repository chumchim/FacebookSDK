using FacebookSDK.Messaging;
using FacebookSDK.Profile;

namespace FacebookSDK.Client;

/// <summary>
/// Facebook Client Implementation
/// </summary>
public class FacebookClient : IFacebookClient
{
    public FacebookClient(
        IFacebookMessaging messaging,
        IFacebookProfile profile)
    {
        Messaging = messaging;
        Profile = profile;
    }

    /// <inheritdoc />
    public IFacebookMessaging Messaging { get; }

    /// <inheritdoc />
    public IFacebookProfile Profile { get; }
}
