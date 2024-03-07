using Piranha.Manager.Services;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public class PageLinkProvider : ILinkProvider
    {
        private readonly PageService _pageService;

        public PageLinkProvider(PageService pageService)
        {
            _pageService = pageService;
        }

        public virtual async Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            var siteList = await _pageService.GetSiteList(siteId);

            return siteList.Items.Select(x => (Link)x);
        }
    }
}
