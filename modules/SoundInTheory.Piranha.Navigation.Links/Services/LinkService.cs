using Piranha;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public class LinkService
    {
        private readonly IApi _api;

        private readonly ILinkProvider[] _linkProviders;

        public LinkService(IApi api, IEnumerable<ILinkProvider> linkProviders)
        {
            _api = api;
            _linkProviders = linkProviders.ToArray();
        }

        public async Task<IEnumerable<Link>> GetLinksAsync(LinkListOptions options)
        {
            var siteId = await GetSiteId(options.SiteId);
            var links = new List<Link>();

            foreach (var provider in _linkProviders)
            {
                var providerLinks = await provider.GetAllAsync(siteId);
                links.AddRange(providerLinks);
            }

            if (options.Search != null)
            {
                links = links.Where(x => x.Text.Contains(options.Search, StringComparison.InvariantCultureIgnoreCase) || x.Url.Contains(options.Search, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            return links;
        }

        protected virtual async Task<Guid> GetSiteId(Guid? selected) => selected ?? (await _api.Sites.GetDefaultAsync()).Id;
    }
}
