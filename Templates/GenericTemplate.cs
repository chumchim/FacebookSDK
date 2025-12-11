using FacebookSDK.Messages;

namespace FacebookSDK.Templates;

/// <summary>
/// Generic Template (Carousel)
/// </summary>
public record GenericTemplate(List<GenericElement> Elements) : IFacebookMessage
{
    public object ToJson() => new
    {
        attachment = new
        {
            type = "template",
            payload = new
            {
                template_type = "generic",
                elements = Elements.Select(e => e.ToJson())
            }
        }
    };
}

/// <summary>
/// Generic Template Element
/// </summary>
public record GenericElement
{
    public string Title { get; init; } = "";
    public string? Subtitle { get; init; }
    public string? ImageUrl { get; init; }
    public IFacebookButton? DefaultAction { get; init; }
    public List<IFacebookButton>? Buttons { get; init; }

    public object ToJson()
    {
        var obj = new Dictionary<string, object>
        {
            ["title"] = Title
        };

        if (!string.IsNullOrEmpty(Subtitle))
            obj["subtitle"] = Subtitle;

        if (!string.IsNullOrEmpty(ImageUrl))
            obj["image_url"] = ImageUrl;

        if (DefaultAction != null)
            obj["default_action"] = DefaultAction.ToJson();

        if (Buttons?.Any() == true)
            obj["buttons"] = Buttons.Select(b => b.ToJson()).ToList();

        return obj;
    }
}
