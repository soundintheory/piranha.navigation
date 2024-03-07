using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.Data.EF.SQLite;
using Piranha.Manager.Editor;
using NavigationMvcExample.Models;
using SoundInTheory.Piranha.Navigation.Rendering;
using SoundInTheory.Piranha.Navigation;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("piranha");

builder.AddPiranha(options =>
{
    /**
     * This will enable automatic reload of .cshtml
     * without restarting the application. However since
     * this adds a slight overhead it should not be
     * enabled in production.
     */
    options.AddRazorRuntimeCompilation = true;

    options.UseCms();
    options.UseManager();

    options.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
    options.UseImageSharp();
    options.UseTinyMCE();
    options.UseMemoryCache();

    options.UseEF<SQLiteDb>(db => db.UseSqlite(connectionString));
    options.UseIdentityWithSeed<IdentitySQLiteDb>(db => db.UseSqlite(connectionString));
    options.UseMenus();
    options.UseLinks();

    // TODO: Test sublink functionality
    //options.AddLinkProvider<SubPageLinkProvider>();

    /**
     * Here you can configure the different permissions
     * that you want to use for securing content in the
     * application.
    options.UseSecurity(o =>
    {
        o.UsePermission("WebUser", "Web User");
    });
     */

    /**
     * Here you can specify the login url for the front end
     * application. This does not affect the login url of
     * the manager interface.
    options.LoginUrl = "login";
     */
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UsePiranha(options =>
{
    // Initialize Piranha
    App.Init(options.Api);

    // Add menus
    App.Modules.Navigation().Menus.Register("primary", "Primary Nav", maxDepth: 2);
    App.Modules.Navigation().Menus.Register("footer", "Footer Links", maxDepth: 1);

    // Test menu hooks
    MenuModule.Hooks.OnRenderMenuItem += (MenuItemRenderContext context) =>
    {
        var currentId = context.App?.GetCurrentItemId();
        if (currentId.HasValue)
        {
            context.Item.Link.Url += "?fromId=" + currentId;

            if (!string.IsNullOrEmpty(context.Item.Link.ContentLink?.TypeId))
            {
                context.Item.Link.Url += "&itemType=" + context.Item.Link.ContentLink.TypeId;
            }
        }
    };

    // Build content types
    new ContentTypeBuilder(options.Api)
        .AddAssembly(typeof(Program).Assembly)
        .Build()
        .DeleteOrphans();

    // Configure Tiny MCE
    EditorConfig.FromFile("editorconfig.json");

    options.UseManager();
    options.UseTinyMCE();
    options.UseIdentity();
    options.UseMenus();
    options.UseLinks();

    // Add navigation
    App.Modules.Navigation().Menus.Register("primary", "Primary Nav", maxDepth: 2);
    App.Modules.Navigation().Menus.Register("footer", "Footer Links", maxDepth: 1);

    App.Blocks.Register<FavouriteWebsiteBlock>();
  
});



app.Run();