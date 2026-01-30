using Piranha.Extend;
using Piranha.Extend.Fields;
using SoundInTheory.Piranha.Navigation.Attributes;
using SoundInTheory.Piranha.Navigation.Models;

namespace NavigationMvcExample.Models.Menu
{
    [MenuItemType(Id = "Link", Title = "My Link Item", ViewPath = "Menu/MyLinkItem")]
    public class MyLinkItem : LinkMenuItem
    {
        /// <summary>
        /// Gets/sets the text content for this menu item.
        /// </summary>
        [Field(Title = "Custom Class")]
        public StringField CustomClass { get; set; }
    }
}
