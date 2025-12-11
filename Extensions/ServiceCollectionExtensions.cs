using FacebookSDK.Client;
using FacebookSDK.Messaging;
using FacebookSDK.Options;
using FacebookSDK.Profile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FacebookSDK.Extensions;

/// <summary>
/// DI Extensions for Facebook SDK
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// เพิ่ม Facebook Client เข้า DI Container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configure">Configure options</param>
    /// <returns>IServiceCollection</returns>
    /// <example>
    /// <code>
    /// services.AddFacebookClient(options => {
    ///     options.PageAccessToken = "your-token";
    ///     options.AppSecret = "your-secret";
    ///     options.VerifyToken = "your-verify-token";
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddFacebookClient(
        this IServiceCollection services,
        Action<FacebookClientOptions> configure)
    {
        // Configure options
        services.Configure(configure);

        // Validate options on startup
        services.PostConfigure<FacebookClientOptions>(options => options.Validate());

        return services.AddFacebookClientCore();
    }

    /// <summary>
    /// เพิ่ม Facebook Client เข้า DI Container จาก Configuration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration section</param>
    /// <returns>IServiceCollection</returns>
    /// <example>
    /// <code>
    /// // appsettings.json:
    /// // "Facebook": {
    /// //   "PageAccessToken": "your-token",
    /// //   "AppSecret": "your-secret",
    /// //   "VerifyToken": "your-verify-token"
    /// // }
    ///
    /// services.AddFacebookClient(configuration.GetSection("Facebook"));
    /// </code>
    /// </example>
    public static IServiceCollection AddFacebookClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind from configuration
        services.Configure<FacebookClientOptions>(configuration);

        // Validate options on startup
        services.PostConfigure<FacebookClientOptions>(options => options.Validate());

        return services.AddFacebookClientCore();
    }

    /// <summary>
    /// เพิ่ม Facebook Client เข้า DI Container จาก Configuration Section Name
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Full configuration</param>
    /// <param name="sectionName">Section name (default: "Facebook")</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddFacebookClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = FacebookClientOptions.SectionName)
    {
        return services.AddFacebookClient(configuration.GetSection(sectionName));
    }

    private static IServiceCollection AddFacebookClientCore(this IServiceCollection services)
    {
        // Register HttpClient
        services.AddHttpClient<IFacebookMessaging, FacebookMessagingService>();
        services.AddHttpClient<IFacebookProfile, FacebookProfileService>();

        // Register client
        services.AddScoped<IFacebookClient, FacebookClient>();

        return services;
    }
}
