using FacebookSDK.Messages;

namespace FacebookSDK.Templates;

/// <summary>
/// Button Template
/// </summary>
public record ButtonTemplate(string Text, List<IFacebookButton> Buttons) : IFacebookMessage
{
    public object ToJson() => new
    {
        attachment = new
        {
            type = "template",
            payload = new
            {
                template_type = "button",
                text = Text,
                buttons = Buttons.Select(b => b.ToJson())
            }
        }
    };
}

/// <summary>
/// Facebook Button Interface
/// </summary>
public interface IFacebookButton
{
    object ToJson();
}

/// <summary>
/// URL Button
/// </summary>
public record UrlButton(string Title, string Url, string? WebviewHeightRatio = null) : IFacebookButton
{
    public object ToJson()
    {
        var obj = new Dictionary<string, object>
        {
            ["type"] = "web_url",
            ["title"] = Title,
            ["url"] = Url
        };

        if (!string.IsNullOrEmpty(WebviewHeightRatio))
            obj["webview_height_ratio"] = WebviewHeightRatio;

        return obj;
    }
}

/// <summary>
/// Postback Button
/// </summary>
public record PostbackButton(string Title, string Payload) : IFacebookButton
{
    public object ToJson() => new
    {
        type = "postback",
        title = Title,
        payload = Payload
    };
}

/// <summary>
/// Call Button
/// </summary>
public record CallButton(string Title, string PhoneNumber) : IFacebookButton
{
    public object ToJson() => new
    {
        type = "phone_number",
        title = Title,
        payload = PhoneNumber
    };
}
