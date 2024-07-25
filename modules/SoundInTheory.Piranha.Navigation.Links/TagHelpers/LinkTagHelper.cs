using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Piranha.AspNetCore.Services;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoundInTheory.Piranha.Navigation.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "link", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class LinkTagHelper : TagHelper
    {
        public LinkField Link { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Link.IsNullOrEmpty())
            {
                output.SuppressOutput();
                return;
            }

            var tag = Link.GetTagBuilder();

            output.MergeAttributes(tag);

            if (output.TagMode == TagMode.SelfClosing && tag.HasInnerHtml)
            {
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.SetHtmlContent(tag.InnerHtml);
            }
        }
    }
}
