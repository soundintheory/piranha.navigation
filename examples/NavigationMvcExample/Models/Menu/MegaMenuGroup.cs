using SoundInTheory.Piranha.Navigation.Attributes;
using SoundInTheory.Piranha.Navigation.Models;

namespace NavigationMvcExample.Models.Menu
{
    [MenuItemType(Title = "Mega Menu Group", AllowedParents = new Type[] { typeof(MegaMenuItem) })]
    public class MegaMenuGroup : StaticMenuItem
    {

    }
}
