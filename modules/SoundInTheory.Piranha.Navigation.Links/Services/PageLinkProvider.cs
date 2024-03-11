using Piranha;
using Piranha.Manager.Services;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public class PageLinkProvider : ILinkProvider
    {
        private readonly IApi _api;

        public PageLinkProvider(IApi api)
        {
            _api = api;
        }

        public virtual async Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            var pages = await _api.Pages.GetAllAsync<PageInfo>();

            return pages.Select(x => (Link)x).Where(x => x != null);
        }
    }
}
