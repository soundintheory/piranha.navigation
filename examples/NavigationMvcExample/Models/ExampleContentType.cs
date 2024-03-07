using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Fields.Settings;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation;

namespace NavigationMvcExample.Models
{
	[ContentGroup(Title = "Example", Icon = "fas fa-file")]
	[ContentType(Title = "Example Content", UseExcerpt = false, UsePrimaryImage = false)]
	public class ExampleContentType : Content<ExampleContentType>
	{
		[Region]
		public ExampleRegion Content { get; set; }

		[Region]
		public List<ExampleRegion> MoreContent { get; set; }

		public class ExampleRegion
		{
			[Field]
			public HtmlField Content { get; set; }

            [Field]
            [ColorFieldSettings(DisallowInput = true)]
            public ColorField FavouiteColor { get; set; }

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