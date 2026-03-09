using Piranha;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<LinkedObject>> GetLinksAsync(LinkListOptions options)
        {
            var siteId = await GetSiteId(options.SiteId);
            var links = new List<LinkedObject>();

            foreach (var provider in _linkProviders)
            {
                var providerLinks = await provider.GetAllAsync(siteId);
                links.AddRange(providerLinks);
            }

            if (options.Search != null)
            {
                links = links
                    .Where(x => x.Link != null && !string.IsNullOrEmpty(x.Link.Url))
                    .Where(x => 
                        x.Link.Text?.Contains(options.Search, StringComparison.InvariantCultureIgnoreCase) == true ||
                        x.Text?.Contains(options.Search, StringComparison.InvariantCultureIgnoreCase) == true ||
                        x.Link.Url.Contains(options.Search, StringComparison.InvariantCultureIgnoreCase))

                    .ToList();
            }

            return links;
        }

        /// <summary>
        /// Resolves a stored link by its ID, routing to the provider that owns the given link type.
        /// Returns null if no matching provider is found or the content does not exist.
        /// </summary>
        public async Task<LinkedObject> GetByIdAsync(string id, string linkType)
        {
            var provider = _linkProviders.FirstOrDefault(p =>
                string.Equals(p.LinkType, linkType, StringComparison.OrdinalIgnoreCase));

            return provider != null
                ? await provider.GetByIdAsync(id).ConfigureAwait(false)
                : null;
        }

        protected virtual async Task<Guid> GetSiteId(Guid? selected) => selected ?? (await _api.Sites.GetDefaultAsync()).Id;
    }
}
