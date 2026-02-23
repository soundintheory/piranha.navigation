using Piranha.Manager.Services;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Services;
using NavigationLinkType = SoundInTheory.Piranha.Navigation.Models.LinkType;

namespace NavigationMvcExample.Services
{
    public class SubPageLinkProvider : ILinkProvider
    {
        private readonly PageService _pageService;

        public SubPageLinkProvider(PageService pageService)
        {
            _pageService = pageService;
        }

        // Sub-links are page-based; PageLinkProvider handles ID resolution for "Page" type.
        public string LinkType => NavigationLinkType.Page;

        public async Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            var siteList = await _pageService.GetSiteList(siteId);

            return siteList.Items.Select(x => Link.FromPageItem(x).SubLink("child-page", "Child Page"));
        }

        public Task<LinkedObject> GetByIdAsync(string id)
        {
            // PageLinkProvider handles resolution for Page-typed links.
            return Task.FromResult<LinkedObject>(null);
        }
    }
}
