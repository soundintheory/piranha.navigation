using Newtonsoft.Json;
using Piranha;
using Piranha.Extend;
using SoundInTheory.Piranha.Navigation.Attributes;
using SoundInTheory.Piranha.Navigation.Extensions;
using SoundInTheory.Piranha.Navigation.Fields;
using SoundInTheory.Piranha.Navigation.Rendering;
using SoundInTheory.Piranha.Navigation.Serializers;
using System;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    /// <summary>
    /// A menu item that represents a clickable link.
    /// </summary>
    [MenuItemType(Id = "Link", Title = "Link", ListTitle = "{{ item.link.text }}", ListType = "{{ item.link.type }} Link", Description = "A clickable link to internal or external content")]
    public class LinkMenuItem : MenuItem
    {
        /// <summary>
        /// Gets/sets the link information for this menu item.
        /// </summary>
        [Field]
        public MenuLinkField Link { get; set; }

        public override Task Init(IApi api)
        {
            if (Link != null)
            {
                return Link.Init(api);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Renders the link menu item to the output
        /// </summary>
        /// <param name="viewModel">The render context containing output and options</param>
        public override void Render(MenuItemViewModel viewModel)
        {
            if (!IsValid)
            {
                return;
            }

            var parentClass = Children?.Count > 0 ? viewModel.Options.ParentLinkClass : "";
            var linkClass = Level > 1 ? viewModel.Options.SubnavLinkClass : viewModel.Options.LinkClass;
            var targetAttr = Link.Attributes != null && Link.Attributes.TryGetValue("target", out var target) && !string.IsNullOrEmpty(target?.ToString()) ? $"target=\"{target}\"" : "";
            var relAttr = Link.Attributes != null && Link.Attributes.TryGetValue("rel", out var rel) && !string.IsNullOrEmpty(rel?.ToString()) ? $" rel=\"{rel}\"" : "";

            if (Link.Type == LinkType.None)
            {
                viewModel.Output.AppendHtml($"<a class=\"{linkClass} level-{Level} {parentClass}\"{relAttr}>{Link.Text}</a>");
            }
            else
            {
                viewModel.Output.AppendHtml($"<a href=\"{Link.Url}\" class=\"{linkClass} level-{viewModel.Item.Level} {parentClass}\" {targetAttr}{relAttr}>{Link.Text}</a>");
            }

            if (viewModel.Recursive)
            {
                viewModel.Renderer.RenderSubnav(viewModel);
            }
        }

        public override bool IsActive(MenuItemViewModel viewModel)
        {
            return 
                (Link?.Id != null && Link.Id == viewModel.App.GetCurrentItemId()) || 
                (Link?.Url != null && Link.Url == viewModel.App.Request?.Url);
        }

        [JsonIgnore]
        public override bool IsValid => Link != null;
    }
}
