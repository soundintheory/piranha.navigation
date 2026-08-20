using Piranha.AttributeBuilder;
using Piranha.Models;

namespace NavigationMvcExample.Models.Sites;

[SiteType(Title = "Main Site")]
public class MainSite : SiteContent<MainSite>
{
}