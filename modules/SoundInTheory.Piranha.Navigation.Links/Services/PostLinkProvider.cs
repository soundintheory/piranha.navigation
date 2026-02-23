using Piranha;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public class PostLinkProvider : ILinkProvider
    {
        private readonly IApi _api;

        public PostLinkProvider(IApi api)
        {
            _api = api;
        }

        public string LinkType => Models.LinkType.Post;

        public virtual async Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            var posts = await _api.Posts.GetAllBySiteIdAsync<PostInfo>(siteId);

            return posts.Select(x => (Link)x).Where(x => x != null);
        }

        public async Task<LinkedObject> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;

            var post = await _api.Posts.GetByIdAsync<PostInfo>(guid).ConfigureAwait(false);
            if (post != null)
                return new LinkedObject { Link = (Link)post, Content = post };

            // Backward-compat fallback: old data may have stored "Post" for what is actually a page
            var page = await _api.Pages.GetByIdAsync<PageInfo>(guid).ConfigureAwait(false);
            if (page != null)
                return new LinkedObject { Link = (Link)page, Content = page };

            return null;
        }
    }
}
