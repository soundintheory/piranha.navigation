using Piranha;
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

        public string LinkType => Models.LinkType.Page;

        public virtual async Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            var pages = await _api.Pages.GetAllAsync<PageInfo>();

            return pages.Select(x => (Link)x).Where(x => x != null);
        }

        public async Task<LinkedObject> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;

            var page = await _api.Pages.GetByIdAsync<PageInfo>(guid).ConfigureAwait(false);
            if (page != null)
                return new LinkedObject { Link = (Link)page, Content = page };

            // Backward-compat fallback: old data may have stored "Page" for what is actually a post
            var post = await _api.Posts.GetByIdAsync<PostInfo>(guid).ConfigureAwait(false);
            if (post != null)
                return new LinkedObject { Link = (Link)post, Content = post };

            return null;
        }
    }
}
