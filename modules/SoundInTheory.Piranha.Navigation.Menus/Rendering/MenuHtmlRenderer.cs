using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.Manager.Localization;
using SoundInTheory.Piranha.Navigation.Extensions;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NavigationModule = SoundInTheory.Piranha.Navigation.MenuModule;

namespace SoundInTheory.Piranha.Navigation.Rendering
{
    public class MenuHtmlRenderer : IMenuRenderer
    {
        private readonly IApplicationService _app;

        public MenuHtmlRenderer(IApplicationService app)
        {
            _app = app;
        }

        public static MenuRenderOptions DefaultOptions { get; private set; } = new MenuRenderOptions
        {
            ListClass = "nav-list",
            ListItemClass = "nav-item",
            LinkClass = "nav-link",
            ParentItemClass = "has-children",
            ParentLinkClass = "",
            SubnavClass = "subnav",
            SubnavListClass = "subnav-list",
            SubnavListItemClass = "subnav-item",
            SubnavLinkClass = "",
            ActiveClass = "active",
            ParentActiveClass = "parent-active"
        };

        public IHtmlContent RenderHtml(Models.Menu menu) => RenderHtml(menu, null);

        public IHtmlContent RenderHtml(Models.Menu menu, Action<MenuRenderOptions> configure)
        {
            var output = new HtmlContentBuilder();
            RenderHtml(menu, output, configure);

            return output;
        }

        public void RenderHtml(Models.Menu menu, IHtmlContentBuilder output, Action<MenuRenderOptions> configure)
        {
            if (menu == null)
            {
                return;
            }

            var options = new MenuRenderOptions();
            var context = new MenuRenderContext
            {
                App = _app,
                Menu = menu,
                Options = options,
                Output = output,
                Level = 1
            };

            configure?.Invoke(options);

            options.SetDefaults(DefaultOptions, context);

            // Set the active state of each item before we render
            var setActive = options.SetActive ?? SetActive;
            
            menu.Items.ForEachRecursive((item) => {
                var itemContext = new MenuItemRenderContext(context) { Item = item, Parent = item.Parent };
                setActive(itemContext);
                if (itemContext.Item.Active)
                {
                    // Tell the parents they have an active child
                    itemContext.Item.GetBreadcrumbs().ToList().ForEach(i => i.ParentActive = true);
                }
            });

            RenderMenu(context);
        }

        /// <summary>
        /// Renders the outer menu element and its contents
        /// </summary>
        protected virtual void RenderMenu(MenuRenderContext context)
        {
            NavigationModule.Hooks.OnRenderMenu?.Invoke(context);

            var container = new TagBuilder("nav");
            container.MergeAttributes(context.Options.ContainerAttributes);
            context.Output.AppendHtml(container.RenderStartTag());

            RenderList(new MenuListRenderContext(context) { Items = context.Menu.Items });

            context.Output.AppendHtml(container.RenderEndTag());
        }

        /// <summary>
        /// Renders lists of menu items within the menu
        /// </summary>
        protected virtual void RenderList(MenuListRenderContext context)
        {
            var listClass = context.Level > 1 ? context.Options.SubnavListClass : context.Options.ListClass;

            context.Output.AppendHtml($"<ul class=\"{listClass} level-{context.Level}\">");

            foreach (var item in context.Items)
            {
                RenderItem(new MenuItemRenderContext(context) { Item = item, Parent = context.Parent });
            }

            context.Output.AppendHtml($"</ul>");
        }

        /// <summary>
        /// Renders list items
        /// </summary>
        protected virtual void RenderItem(MenuItemRenderContext context)
        {
            NavigationModule.Hooks.OnRenderMenuItem?.Invoke(context);

            if (context.Item == null || context.Item.Hidden)
            {
                return;
            }

            var activeClass = context.ActiveClass;
            var itemClass = context.ListItemClass;
            var parentClass = context.ParentClass;

            context.Output.AppendHtml($"<li class=\"{itemClass} level-{context.Level} {activeClass} {parentClass}\">");

            RenderLink(context);

            if (context.Item?.Children?.Count > 0 && context.Level < context.Menu.Settings.MaxDepth)
            {
                RenderSubnav(new MenuListRenderContext(context) { Items = context.Item?.Children, Parent = context.Item, Level = context.Level + 1 });
            }

            context.Output.AppendHtml($"</li>");
        }

        /// <summary>
        /// Renders a subnav within the menu
        /// </summary>
        protected virtual void RenderSubnav(MenuListRenderContext context)
        {
            context.Output.AppendHtml($"<nav class=\"{context.Options.SubnavClass} level-{context.Level}\">");

            RenderList(context);

            context.Output.AppendHtml($"</nav>");
        }

        /// <summary>
        /// Renders an item link
        /// </summary>
        protected virtual void RenderLink(MenuItemRenderContext context)
        {
            var parentClass = context.Item?.Children?.Count > 0 ? context.Options.ParentLinkClass : "";
            var linkClass = context.Level > 1 ? context.Options.SubnavLinkClass : context.Options.LinkClass;
            var targetAttr = context.Item.Link.Attributes != null && context.Item.Link.Attributes.TryGetValue("target", out var target) && !string.IsNullOrEmpty(target?.ToString()) ? $"target=\"{target}\"" : "";

            var relAttr = context.Item.Link.Attributes != null && context.Item.Link.Attributes.TryGetValue("rel", out var rel) && !string.IsNullOrEmpty(rel?.ToString()) ? $" rel=\"{rel}\"" : "";

            if (context.Item.Link.Type == LinkType.None)
            {
                context.Output.AppendHtml($"<a class=\"{linkClass} level-{context.Level} {parentClass}\"{relAttr}>{context.Item.Link.Text}</a>");
            }
            else
            {
                context.Output.AppendHtml($"<a href=\"{context.Item.Link.Url}\" class=\"{linkClass} level-{context.Level} {parentClass}\" {targetAttr}{relAttr}>{context.Item.Link.Text}</a>");
            }
        }

        /// <summary>
        /// Checks whether an item should be active and sets the relevant booleans
        /// </summary>
        protected void SetActive(MenuItemRenderContext context)
        {
            var activeItemId = _app.GetCurrentItemId();
            context.Item.Active = (context.Item.Link.Id.HasValue && context.Item.Link.Id == activeItemId) || context.Item.Link.Url == _app.Request?.Url;
        }
    }

    public class MenuRenderContext
    {
        public MenuRenderContext() { }

        public MenuRenderContext(MenuRenderContext context)
        {
            App = context.App;
            Menu = context.Menu;
            Options = context.Options;
            Output = context.Output;
            Level = context.Level;
        }
        
        public Models.Menu Menu { get; internal set; }

        public IApplicationService App { get; set; }

        public MenuRenderOptions Options { get; internal set; }

        public IHtmlContentBuilder Output { get; internal set; }

        public int Level { get; internal set; }
    }

    public class MenuListRenderContext : MenuRenderContext
    {
        public MenuListRenderContext() { }

        public MenuListRenderContext(MenuRenderContext context) : base(context) { }

        public IList<MenuItem> Items { get; internal set; }

        public MenuItem Parent { get; internal set; }
    }

    public class MenuItemRenderContext : MenuRenderContext
    {
        public MenuItemRenderContext() { }

        public MenuItemRenderContext(MenuRenderContext context) : base(context) { }

        public MenuItem Item { get; internal set; }

        public MenuItem Parent { get; internal set; }

        public string ListItemClass => Level > 1 ? Options.SubnavListItemClass : Options.ListItemClass;

        public string ParentClass => Item?.Children?.Count > 0 ? Options.ParentItemClass : "";

        public string ActiveClass
        {
            get
            {
                if (Item?.Active == true)
                {
                    return Options.ActiveClass;
                }

                if (Item?.ParentActive == true)
                {
                    return Options.ParentActiveClass;
                }

                return string.Empty;
            }
        }
    }

    public class MenuRenderOptions
    {
        public IDictionary<string, object> ContainerAttributes { get; internal set; }
        public string ListClass { get; set; }
        public string ListItemClass { get; set; }
        public string LinkClass { get; set; }
        public string ParentItemClass { get; set; }
        public string ParentLinkClass { get; set; }
        public string SubnavClass { get; set; }
        public string SubnavListClass { get; set; }
        public string SubnavListItemClass { get; set; }
        public string SubnavLinkClass { get; set; }
        public string ActiveClass { get; set; }
        public string ParentActiveClass { get; set; }
        public Action<MenuItemRenderContext> SetActive { get; set; }

        public void SetDefaults(MenuRenderOptions defaults, MenuRenderContext context)
        {
            ListClass ??= defaults.ListClass;
            ListItemClass ??= defaults.ListItemClass;
            LinkClass ??= defaults.LinkClass;
            ParentItemClass ??= defaults.ParentItemClass;
            ParentLinkClass ??= defaults.ParentLinkClass;
            SubnavClass ??= defaults.SubnavClass;
            SubnavListClass ??= defaults.SubnavListClass;
            SubnavListItemClass ??= defaults.SubnavListItemClass;
            SubnavLinkClass ??= defaults.SubnavLinkClass;
            ActiveClass ??= defaults.ActiveClass;
            ParentActiveClass ??= defaults.ParentActiveClass;
            ContainerAttributes ??= new Dictionary<string, object>();
            
            if (!ContainerAttributes.ContainsKey("class"))
            {
                ContainerAttributes["class"] = $"nav nav-{context.Menu.Slug}";
            }

            if (!ContainerAttributes.ContainsKey("id"))
            {
                ContainerAttributes["id"] = $"nav-{context.Menu.Slug}";
            }
        }
    }
}
