using SoundInTheory.Piranha.Navigation.Attributes;
using SoundInTheory.Piranha.Navigation.Models;

namespace NavigationMvcExample.Models.Menu
{
    [MenuItemType(Title = "Menu Group", MaxDepth = 1, AllowedParents = new Type[] { typeof(SoundInTheory.Piranha.Navigation.Models.Menu), typeof(MegaMenuItem) })]
    public class MegaMenuGroup : StaticMenuItem
    {

    }
}
