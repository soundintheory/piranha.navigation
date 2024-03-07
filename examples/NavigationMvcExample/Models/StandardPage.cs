using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Fields.Settings;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation;

namespace NavigationMvcExample.Models
{
    [PageType(Title = "Standard page")]
    public class StandardPage  : Page<StandardPage>
    {
        [Region]
        public ExampleRegion Content { get; set; }

        public class ExampleRegion
        {
            [Field]
            [ColorFieldSettings(DisallowInput = true)]
            public ColorField FavouriteColor { get; set; }

            [Field]
            public NumberField FavouriteNumber { get; set; }

            [Field]
            public PageField PageField { get; set; }

            [Field]
            public PostField PostField { get; set; }

            [Field]
            public LinkField LinkField { get; set; }
        }
    }
}