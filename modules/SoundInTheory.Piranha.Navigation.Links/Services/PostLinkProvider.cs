using Microsoft.AspNetCore.Http;
using Piranha;
using Piranha.AspNetCore.Services;
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

        private readonly ApplicationServiceAccessor _appAccessor;

        public PostLinkProvider(IApi api, ApplicationServiceAccessor appAccessor)
        {
            _api = api;
            _appAccessor = appAccessor;
        }

        public string LinkType => Models.LinkType.Post;

        public virtual async Task<IEnumerable<LinkedObject>> GetAllAsync(Guid siteId)
        {
            var posts = await _api.Posts.GetAllBySiteIdAsync<PostInfo>(siteId);

            return posts.Select(x => new LinkedObject(Link.FromPost(x, _appAccessor.ApplicationService))).Where(x => x.Link != null);
        }

        public async Task<LinkedObject> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;

            var post = await _api.Posts.GetByIdAsync<PostInfo>(guid).ConfigureAwait(false);
            if (post != null)
                return new LinkedObject(Link.FromPost(post, _appAccessor.ApplicationService)) { Content = post };

            // Backward-compat fallback: old data may have stored "Post" for what is actually a page
            var page = await _api.Pages.GetByIdAsync<PageInfo>(guid).ConfigureAwait(false);
            if (page != null)
                return new LinkedObject(Link.FromPage(page, _appAccessor.ApplicationService)) { Content = page };

            return null;
        }
    }
}
