using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Piranha;
using Piranha.AspNetCore;
using Piranha.AspNetCore.Models;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Services;
using Piranha.Models;
using Piranha.Runtime;
using SoundInTheory.Piranha.Navigation;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Models.Content;
using SoundInTheory.Piranha.Navigation.Rendering;
using SoundInTheory.Piranha.Navigation.Repositories;
using SoundInTheory.Piranha.Navigation.Serializers;
using SoundInTheory.Piranha.Navigation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public static class MenuExtensions
{
    /// <summary>
    /// Adds the Navigation module.
    /// </summary>
    public static PiranhaServiceBuilder UseMenus(this PiranhaServiceBuilder serviceBuilder)
    {
        serviceBuilder.Services.AddMenus();
        return serviceBuilder;
    }

    /// <summary>
    /// Uses the Navigation module.
    /// </summary>
    /// <param name="applicationBuilder">The current application builder</param>
    /// <returns>The builder</returns>
    public static PiranhaApplication UseMenus(this PiranhaApplication applicationBuilder)
    {
        applicationBuilder.Builder.UseMenus();

        var builder = new ContentTypeBuilder(applicationBuilder.Api)
            .AddType(typeof(NavigationMenu));
        builder.Build();

        App.Fields.Register<MenuSettingsField>();
        App.Fields.Register<MenuItemsField>();

        App.Serializers.Register<MenuSettingsField>(new SimpleFieldSerializer<MenuSettingsField, MenuSettings>());
        App.Serializers.Register<MenuItemsField>(new SimpleFieldSerializer<MenuItemsField, List<MenuItem>>());

        return applicationBuilder;
    }

    /// <summary>
    /// Uses the Navigation.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UseMenus(this IApplicationBuilder builder)
    {
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = FileProvider,
            RequestPath = "/manager/Navigation/assets"
        });
    }

    /// <summary>
    /// Adds the Navigation module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddMenus(this IServiceCollection services)
    {
        // Register services
        services.TryAddScoped<IMenuService, MenuService>();
        services.TryAddScoped<IMenuRenderer, MenuHtmlRenderer>();
        services.TryAddScoped<IMenuRepository, MenuRepository>();
        services.AddLinkServices();

        // Add the Navigation module
        App.Modules.Register<MenuModule>();

        // Setup authorization policies
        services.AddAuthorization(o =>
        {
            // Read menus
            o.AddPolicy(Permissions.Menus, policy =>
            {
                policy.RequireClaim(Permissions.Menus, Permissions.Menus);
            });

            // Edit menu items
            o.AddPolicy(Permissions.MenuItemsEdit, policy =>
            {
                policy.RequireClaim(Permissions.Menus, Permissions.Menus);
                policy.RequireClaim(Permissions.MenuItemsEdit, Permissions.MenuItemsEdit);
            });

            // Delete menu items
            o.AddPolicy(Permissions.MenusItemsDelete, policy =>
            {
                policy.RequireClaim(Permissions.Menus, Permissions.Menus);
                policy.RequireClaim(Permissions.MenusItemsDelete, Permissions.MenusItemsDelete);
            });
        });

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Static accessor to Navigation module if it is registered in the Piranha application.
    /// </summary>
    /// <param name="modules">The available modules</param>
    /// <returns>The Navigation module</returns>
    public static MenuModule Navigation(this Piranha.Runtime.AppModuleList modules)
    {
        return MenuModule.Instance;
    }

    /// <summary>
    /// Static accessor to navigation hooks
    /// </summary>
    /// <returns>The Navigation module</returns>
    public static NavigationHooks Navigation(this HookManager hookManager)
    {
        return MenuModule.Hooks;
    }

    private static IFileProvider FileProvider
    {
        get
        {
            if (IsDebugBuild)
            {
                return new PhysicalFileProvider(GetProjectPath("assets"));
            }

            return new EmbeddedFileProvider(typeof(MenuModule).Assembly, "SoundInTheory.Piranha.Navigation.assets");
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
