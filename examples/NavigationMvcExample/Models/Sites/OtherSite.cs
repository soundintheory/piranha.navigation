using Piranha.AttributeBuilder;
using Piranha.Models;

namespace NavigationMvcExample.Models.Sites;

[SiteType(Title = "Other Site")]
public class OtherSite : SiteContent<OtherSite>
{
}