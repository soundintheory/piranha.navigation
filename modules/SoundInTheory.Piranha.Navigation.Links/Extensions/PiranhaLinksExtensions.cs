using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Piranha;
using Piranha.AspNetCore;
using SoundInTheory.Piranha.Navigation;
using SoundInTheory.Piranha.Navigation.Serializers;
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

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Uses the LinkField.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UseLinks(this IApplicationBuilder builder)
    {
        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = FileProvider,
            RequestPath = "/manager/Links/assets"
        });

        App.Fields.Register<LinkField>();
        App.Blocks.Register<LinkBlock>();
        App.Serializers.Register<LinkField>(new LinkFieldSerializer());
        App.Modules.Manager().Scripts.Add("~/manager/Links/assets/js/link-field.js");
        App.Modules.Manager().Styles.Add("~/manager/Links/assets/css/link-field.css");
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

            return new EmbeddedFileProvider(typeof(LinksModule).Assembly, "SoundInTheory.Piranha.assets");
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
