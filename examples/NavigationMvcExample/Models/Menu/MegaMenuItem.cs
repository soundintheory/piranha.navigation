using Piranha.Extend.Fields;
using SoundInTheory.Piranha.Navigation.Attributes;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Rendering;

namespace NavigationMvcExample.Models.Menu
{
    /// <summary>
    /// Example custom mega menu item type
    /// </summary>
    [MenuItemType(Title = "Mega Menu", ViewPath = "Menu/MegaMenuItem", MaxLevel = 1, MaxDepth = 2, AllowedChildren = new Type[] { typeof(MegaMenuGroup) })]
    public class MegaMenuItem : StaticMenuItem
    {
        public List<MegaMenuGroup> Groups => Children.OfType<MegaMenuGroup>().Where(x => x.Children.Any()).ToList();
    }
}
