using Piranha.Extend;
using Piranha.Extend.Fields;
using SoundInTheory.Piranha.Navigation;

namespace NavigationMvcExample.Models
{
    [BlockType(Name = "Favourite Website", Category = "Custom", Icon = "fas fa-link", IsGeneric = true)]
    public class FavouriteWebsiteBlock : Block
    {
        public LinkField Link { get; set; }
        public TextField ReasonField { get; set; }
    }
}
