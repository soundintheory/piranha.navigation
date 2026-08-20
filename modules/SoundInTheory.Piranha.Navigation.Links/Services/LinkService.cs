using Piranha;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var allSites = (await _api.Sites.GetAllAsync()).ToList();
            var sites = ResolveSites(options.SiteIds, allSites);
            var links = new List<LinkedObject>();

            foreach (var provider in _linkProviders)
            {
                foreach (var site in sites)
                {
                    var providerLinks = (await provider.GetAllAsync(site.Id)).ToList();
                    
                    if (sites.Count > 1)
                    {
                        // Automatically prefix with the site name when there is more than one site in the mix
                        providerLinks.ForEach(link =>
                        {
                            link.Text = $"{site.Title} > {link.Text}";
                        });
                    }

                    links.AddRange(providerLinks);
                }
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

        /// <summary>
        /// Resolve a list of site ids into site objects
        /// If the provided array is empty, this will resolve to all of the sites
        /// </summary>
        protected virtual List<Site> ResolveSites(Guid[] selected, List<Site> allSites)
        {
            if (selected == null || selected.Length == 0)
            {
                return allSites;
            }

            var filteredSites = allSites.Where(x => selected.Contains(x.Id)).ToList();

            return filteredSites.Count > 0 ? filteredSites : allSites;
        }
    }
}
