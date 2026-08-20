using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Piranha;
using Piranha.AspNetCore;
using SoundInTheory.Piranha.Navigation;
using SoundInTheory.Piranha.Navigation.Extensions;
using SoundInTheory.Piranha.Navigation.Serializers;
using SoundInTheory.Piranha.Navigation.Services;
using System.IO;
using System.Runtime.CompilerServices;

public static class PiranhaLinksExtensions
{
    /// <summary>
    /// Adds the LinkField module.
    /// </summary>
    /// <param name="serviceBuilder"></param>
    /// <returns></returns>
    public static PiranhaServiceBuilder UseLinks(this PiranhaServiceBuilder serviceBuilder)
    {
        serviceBuilder.Services.AddLinks();

        return serviceBuilder;
    }

    /// <summary>
    /// Uses the LinkField module.
    /// </summary>
    /// <param name="applicationBuilder">The current application builder</param>
    /// <returns>The builder</returns>
    public static PiranhaApplicationBuilder UseLinks(this PiranhaApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Builder.UseLinks();

        return applicationBuilder;
    }

    /// <summary>
    /// Adds the LinkField module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddLinks(this IServiceCollection services)
    {
        services.AddLinkServices();

        // Add the LinkField module
        Piranha.App.Modules.Register<LinksModule>();

        // Clear site related caches when needed
        Piranha.App.Hooks.Site.RegisterOnAfterSave(x => PiranhaAppServiceExtensions.ClearCache());
        Piranha.App.Hooks.Site.RegisterOnAfterDelete(x => PiranhaAppServiceExtensions.ClearCache());

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Uses the LinkField.
    /// </summary>
    public static IApplicationBuilder UseLinks(this IApplicationBuilder builder, string defaultBaseUrl = null)
    {
        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = FileProvider,
            RequestPath = "/manager/Navigation/assets"
        });

        var assetVersion = Utils.GetAssemblyVersionHash(typeof(LinksModule).Assembly);

        App.Fields.Register<LinkField>();
        App.Blocks.Register<LinkBlock>();
        App.Serializers.Register<LinkField>(new LinkFieldSerializer());
        App.Modules.Manager().Scripts.Add($"~/manager/Navigation/assets/js/link-field.js?v={assetVersion}");
        App.Modules.Manager().Styles.Add($"~/manager/Navigation/assets/css/link-field.css?v={assetVersion}");

        if (string.IsNullOrWhiteSpace(defaultBaseUrl))
        {
            defaultBaseUrl = builder.ApplicationServices.GetRequiredService<IConfiguration>().GetValue<string>("BaseUrl") ?? string.Empty;
        }

        // Set the default base url so that the application service can be used outside of an http context
        ApplicationServiceAccessor.DefaultBaseUrl = defaultBaseUrl;

        return builder;
    }

    /// <summary>
    /// Static accessor to LinkField module if it is registered in the Piranha application.
    /// </summary>
    /// <param name="modules">The available modules</param>
    /// <returns>The LinkField module</returns>
    public static LinksModule LinkField(this Piranha.Runtime.AppModuleList modules)
    {
        return modules.Get<LinksModule>();
    }

    private static IFileProvider FileProvider
    {
        get
        {
            if (IsDebugBuild)
            {
                return new PhysicalFileProvider(GetProjectPath("assets"));
            }

            return new EmbeddedFileProvider(typeof(LinksModule).Assembly, "SoundInTheory.Piranha.Navigation.assets");
        }
    }

    private static string GetProjectPath(string path = null)
    {
        var filePath = GetCurrentFilePath() ?? "";
        var dir = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;

        if (!string.IsNullOrWhiteSpace(path))
        {
            return Path.Join(dir, path);
        }

        return dir;
    }

    private static string GetCurrentFilePath([CallerFilePath] string callerFilePath = null) => callerFilePath;

    private static bool IsDebugBuild
    {
        get
        {
#if DEBUG
            return true;
#else
                return false;
#endif
        }
    }
}
