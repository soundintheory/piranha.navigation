using Piranha.Manager.Services;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Services;

namespace NavigationMvcExample.Services
{
    public class SubPageLinkProvider : ILinkProvider
    {
        private readonly PageService _pageService;

        public SubPageLinkProvider(PageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            var siteList = await _pageService.GetSiteList(siteId);

            return siteList.Items.Select(x => Link.FromPageItem(x).SubLink("child-page", "Child Page"));
        }
    }
}
