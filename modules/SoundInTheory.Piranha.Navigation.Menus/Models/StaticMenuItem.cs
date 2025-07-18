using Microsoft.AspNetCore.Components.RenderTree;
using Newtonsoft.Json;
using Piranha.Extend;
using Piranha.Extend.Fields;
using SoundInTheory.Piranha.Navigation.Attributes;
using SoundInTheory.Piranha.Navigation.Rendering;

namespace SoundInTheory.Piranha.Navigation.Models
{
    /// <summary>
    /// A menu item that represents static text (non-clickable).
    /// </summary>
    [MenuItemType(Id = "Static", Title = "Static Text", ListTitle = "{{ item.text.value }}", Description = "Non-clickable text for section headers or labels")]
    public class StaticMenuItem : MenuItem
    {
        /// <summary>
        /// Gets/sets the text content for this menu item.
        /// </summary>
        [Field]
        public StringField Text { get; set; }

        /// <summary>
        /// Renders the static menu item to the output
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

            viewModel.Output.AppendHtml($"<span class=\"{linkClass} level-{Level} {parentClass}\">{Text.Value}</span>");

            if (viewModel.Recursive)
            {
                viewModel.Renderer.RenderSubnav(viewModel);
            }
        }

        [JsonIgnore]
        public override bool IsValid => Text != null && !string.IsNullOrEmpty(Text.Value);
    }
}
