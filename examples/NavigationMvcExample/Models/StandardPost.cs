using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Fields.Settings;
using Piranha.Models;

namespace NavigationMvcExample.Models
{
    [PostType(Title = "Standard post")]
    public class StandardPost  : Post<StandardPost>
    {
        /// <summary>
        /// Gets/sets the available comments if these
        /// have been loaded from the database.
        /// </summary>
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();


        public class StandardPostRegion
        {
            [Field]
            [ColorFieldSettings(DisallowInput = true, DefaultValue = "#ffffff")]
            public ColorField TestColourField { get; set; }

            [Field]
            public StringField Test { get; set; }
        }

        [Region]
        public StandardPostRegion TestRegion { get; set; }

        [Region]
        public List<StandardPostRegion> TestRegionList { get; set; }
        

    }
}