using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
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
    public class DefaultMenuRenderer : IMenuRenderer
    {
        public IHtmlContentBuilder Output { get; set; }

        private readonly IApplicationService _app;

        private readonly IHtmlHelper _htmlHelper;

        public DefaultMenuRenderer(IApplicationService app, IHtmlHelper htmlHelper)
        {
            _app = app;
            _htmlHelper = htmlHelper;
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
            ParentActiveClass = "parent-active",
            TagName = "nav"
        };

        public virtual IHtmlContent GetHtml(Models.Menu menu) => GetHtml(menu, null);

        public virtual IHtmlContent GetHtml(Models.Menu menu, Action<MenuRenderOptions> configure)
        {
            Output = new HtmlContentBuilder();
            Render(menu, configure);
            return Output;
        }

        private IHtmlContent OutputHtml<T>(T viewModel, Action<T> action) where T : MenuViewModel
        {
            var originalOutput = viewModel.Output;
            var output = viewModel.Output = new HtmlContentBuilder();
            action(viewModel);
            viewModel.Output = originalOutput;
            return output;
        }

        public virtual void Render(Models.Menu menu, Action<MenuRenderOptions> configure = null)
        {
            if (menu == null)
            {
                return;
            }

            var options = new MenuRenderOptions();
            var context = new MenuViewModel
            {
                Renderer = this,
                App = _app,
                Menu = menu,
                Options = options,
                Output = Output
            };

            configure?.Invoke(options);

            options.SetDefaults(DefaultOptions, context);

            // Set the active state of each item before we render
            var setActive = options.SetActive ?? SetActive;
            
            menu.Items.ForEachRecursive((item) => {
                var itemContext = new MenuItemViewModel(item, context);
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
        protected virtual void RenderMenu(MenuViewModel context)
        {
            NavigationModule.Hooks.OnRenderMenu?.Invoke(context);

            var container = new TagBuilder(context.Options.TagName);
            container.MergeAttributes(context.Options.ContainerAttributes);
            context.Output.AppendHtml(container.RenderStartTag());

            // Try and render using a view
            if (_htmlHelper != null && !string.IsNullOrEmpty(context.Options.View))
            {
                try
                {
                    var content = _htmlHelper.Partial(context.Options.View, context);
                    context.Output.AppendHtml(content);
                }
                catch (InvalidOperationException)
                {
                    // The view doesn't exist so just fall back
                    RenderList(new MenuListViewModel(context) { Items = context.Menu.Items });
                }
            }
            else
            {
                RenderList(new MenuListViewModel(context) { Items = context.Menu.Items });
            }

            context.Output.AppendHtml(container.RenderEndTag());
        }

        /// <summary>
        /// Renders lists of menu items within the menu
        /// </summary>
        public IHtmlContent List(MenuListViewModel context, Action<MenuRenderOptions> configure = null)
        {
            return OutputHtml(context, vm => RenderList(vm, configure));
        }

        /// <summary>
        /// Renders lists of menu items within the menu
        /// </summary>
        public virtual void RenderList(MenuListViewModel context, Action<MenuRenderOptions> configure = null)
        {
            if (context.Items == null || !context.Items.Any(i => i != null && i.IsValid && !i.Hidden))
            {
                return;
            }

            context.Configure(configure);

            var listClass = context.Parent != null ? context.Options.SubnavListClass : context.Options.ListClass;

            context.Output.AppendHtml($"<ul class=\"{listClass} level-{(context.Parent?.Level ?? 0) + 1}\">");

            foreach (var item in context.Items)
            {
                RenderListItem(new MenuItemViewModel(item, context));
            }

            context.Output.AppendHtml($"</ul>");
        }

        /// <summary>
        /// Renders a list item
        /// </summary>
        public IHtmlContent ListItem(MenuItemViewModel context, Action<MenuRenderOptions> configure = null)
        {
            return OutputHtml(context, vm => RenderListItem(vm, configure));
        }

        /// <summary>
        /// Renders a list item
        /// </summary>
        public virtual void RenderListItem(MenuItemViewModel context, Action<MenuRenderOptions> configure = null)
        {
            NavigationModule.Hooks.OnRenderMenuItem?.Invoke(context);

            if (context.Item == null || !context.Item.IsValid || context.Item.Hidden)
            {
                return;
            }

            context.Configure(configure);

            context.Output.AppendHtml($"<li class=\"{context.ListItemClass} level-{context.Item.Level} {context.ActiveClass} {context.ParentClass}\">");

            RenderItem(context);

            context.Output.AppendHtml($"</li>");
        }

        public IHtmlContent Item(MenuItemViewModel context, Action<MenuRenderOptions> configure = null)
        {
            return OutputHtml(context, vm => RenderItem(vm, configure));
        }

        /// <summary>
        /// Renders the content of an item
        /// </summary>
        public virtual void RenderItem(MenuItemViewModel context, Action<MenuRenderOptions> configure = null)
        {
            if (context.Item == null || !context.Item.IsValid || context.Item.Hidden)
            {
                return;
            }

            context.Configure(configure);

            // Try and render using a view
            if (_htmlHelper != null && !string.IsNullOrEmpty(context.Definition?.ViewPath))
            {
                try
                {
                    var content = _htmlHelper.Partial(context.Definition.ViewPath, MakeGenericViewModel(context));
                    context.Output.AppendHtml(content);
                }
                catch (InvalidOperationException)
                {
                    // The view doesn't exist so just fall back
                    context.Item.Render(context);
                }
            }
            else
            {
                context.Item.Render(context);
            }
        }

        /// <summary>
        /// Renders a subnav within the menu
        /// </summary>
        public IHtmlContent Subnav(MenuItemViewModel context, Action<MenuRenderOptions> configure = null)
        {
            return OutputHtml(context, vm => RenderSubnav(vm, configure));
        }

        /// <summary>
        /// Renders a subnav within the menu
        /// </summary>
        public virtual void RenderSubnav(MenuItemViewModel context, Action<MenuRenderOptions> configure = null)
        {
            var listContext = new MenuListViewModel(context) { Items = context.Item?.Children, Parent = context.Item };

            if (listContext.Items == null || !listContext.Items.Any(i => i != null && i.IsValid && !i.Hidden))
            {
                return;
            }

            listContext.Configure(configure);

            context.Output.AppendHtml($"<nav class=\"{listContext.Options.SubnavClass} level-{context.Item.Level + 1}\">");
            RenderList(listContext);
            context.Output.AppendHtml($"</nav>");
        }

        /// <summary>
        /// Checks whether an item should be active and sets the relevant booleans
        /// </summary>
        protected void SetActive(MenuItemViewModel context)
        {
            context.Item.Active = context.Item.IsActive(context);
        }

        private object MakeGenericViewModel(MenuItemViewModel model)
        {
            var itemType = model.Item.GetType();
            var genericType = typeof(MenuItemViewModel<>).MakeGenericType(itemType);
            return Activator.CreateInstance(genericType, model.Item, model);
        }
    }

    public class MenuViewModel
    {
        public MenuViewModel() { }

        public MenuViewModel(MenuViewModel context)
        {
            Renderer = context.Renderer;
            App = context.App;
            Menu = context.Menu;
            Options = context.Options.Copy();
            Output = context.Output;
        }

        public void Configure(Action<MenuRenderOptions> action)
        {
            action?.Invoke(this.Options);
        }

        public IMenuRenderer Renderer { get; internal set; }

        public Models.Menu Menu { get; internal set; }

        public IApplicationService App { get; set; }

        public MenuRenderOptions Options { get; internal set; }

        public IHtmlContentBuilder Output { get; internal set; }
    }

    public class MenuListViewModel : MenuViewModel
    {
        public MenuListViewModel() { }

        public MenuListViewModel(MenuViewModel context) : base(context) { }

        public IList<MenuItem> Items { get; internal set; }

        public MenuItem Parent { get; internal set; }
    }

    public class MenuItemViewModel : MenuViewModel
    {
        public MenuItemViewModel() { }

        public MenuItemViewModel(MenuItem item, MenuViewModel context) : base(context)
        {
            Item = item;
            Definition = MenuModule.Instance.MenuItems[item._TypeId];
        }

        public MenuItem Item { get; internal set; }

        public MenuItemDefinition Definition { get; internal set; }

        public bool Recursive { get; set; } = true;

        public string ListItemClass => (Item.Level > 1 ? Options.SubnavListItemClass : Options.ListItemClass) + " " + (Definition?.CssClass ?? "");

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

    public class MenuItemViewModel<T> : MenuItemViewModel where T : MenuItem
    {
        public MenuItemViewModel(T item, MenuViewModel context) : base(item, context) { }

        public new T Item {
            get => base.Item as T;
            protected set => base.Item = value;
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
        public string View { get; set; }
        public string ViewPrefix { get; set; }
        public string TagName { get; set; } 
        public Action<MenuItemViewModel> SetActive { get; set; }

        public void SetDefaults(MenuRenderOptions defaults, MenuViewModel context)
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
            TagName ??= defaults.TagName;
            
            if (!ContainerAttributes.ContainsKey("class"))
            {
                ContainerAttributes["class"] = $"nav nav-{context.Menu.Slug}";
            }

            if (!ContainerAttributes.ContainsKey("id"))
            {
                ContainerAttributes["id"] = $"nav-{context.Menu.Slug}";
            }
        }

        public MenuRenderOptions Copy()
        {
            return new MenuRenderOptions
            {
                ContainerAttributes = new Dictionary<string, object>(ContainerAttributes),
                ListClass = ListClass,
                ListItemClass = ListItemClass,
                LinkClass = LinkClass,
                ParentItemClass = ParentItemClass,
                ParentLinkClass = ParentLinkClass,
                SubnavClass = SubnavClass,
                SubnavListClass = SubnavListClass,
                SubnavListItemClass = SubnavListItemClass,
                SubnavLinkClass = SubnavLinkClass,
                ActiveClass = ActiveClass,
                ParentActiveClass = ParentActiveClass
            };
        }
    }
}
