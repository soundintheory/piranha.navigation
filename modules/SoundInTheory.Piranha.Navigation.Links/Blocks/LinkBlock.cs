using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation
{
    [BlockType(Name = "Link", Category = "References", Icon = "fas fa-link", IsGeneric = true)]
    public class LinkBlock : Block
    {
        public StringField Title { get; set; }

        public TextField Description { get; set; }

        public ImageField Image { get; set; }

        public LinkField Link { get; set; }

        public bool HasLink => !Link.IsNullOrEmpty();

        public bool HasImage => Image?.HasValue == true || Link?.ContentInfo?.PrimaryImage?.HasValue == true;

        public override string GetTitle() => !string.IsNullOrEmpty(Title?.Value) ? Title.Value : Link?.ContentInfo?.Title;

        public string GetDescription() => !string.IsNullOrEmpty(Description?.Value) ? Description.Value : Link?.ContentInfo?.Excerpt;

        public ImageField GetImage() => Image?.HasValue == true ? Image : Link?.ContentInfo?.PrimaryImage;
    }

}