using Microsoft.AspNetCore.Razor.TagHelpers;
using Piranha;
using Piranha.AspNetCore.Services;
using SoundInTheory.Piranha.Navigation.Rendering;
using SoundInTheory.Piranha.Navigation.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.TagHelpers
{
    [HtmlTargetElement("menu", Attributes = "slug")]
    public class MenuTagHelper : TagHelper
    {
        private readonly IMenuService _menuService;

        private readonly IMenuRenderer _menuRenderer;

        private readonly IApplicationService _app;

        public MenuTagHelper(IMenuService menuService, IMenuRenderer menuRenderer, IApplicationService app)
        {
            _menuService = menuService;
            _menuRenderer = menuRenderer;
            _app = app;
        }

        public string Slug { get; set; }

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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var menu = await _menuService.GetBySlug(_app.Site.Id, Slug);

            if (menu == null)
            {
                output.SuppressOutput();
                return;
            }

            // Let the menu renderer do the outer tag
            output.TagName = null;

            // Allow all attributes to pass through to the menu container except the custom ones
            var excludeAttributes = new string[] { "slug", "list-class", "list-item-class", "link-class", "subnav-class", "subnav-list-class", "subnav-list-item-class", "subnav-link-class", "active-class", "parent-active-class" };
            var containerAttributes = context.AllAttributes
                .Where(x => !excludeAttributes.Contains(x.Name))
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x.First().Value);

            _menuRenderer.RenderHtml(
                menu,
                output.Content,
                o =>
                {
                    o.ContainerAttributes = containerAttributes;
                    o.ListClass = ListClass;
                    o.ListItemClass = ListItemClass;
                    o.LinkClass = LinkClass;
                    o.ParentItemClass = ParentItemClass;
                    o.ParentLinkClass = ParentLinkClass;
                    o.SubnavClass = SubnavClass;
                    o.SubnavListClass = SubnavListClass;
                    o.SubnavListItemClass = SubnavListItemClass;
                    o.SubnavLinkClass = SubnavLinkClass;
                    o.ActiveClass = ActiveClass;
                    o.ParentActiveClass = ParentActiveClass;
                });
        }
    }
}
