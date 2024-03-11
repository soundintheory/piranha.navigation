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
    public class PostLinkProvider : ILinkProvider
    {
        private readonly IApi _api;

        public PostLinkProvider(IApi api)
        {
            _api = api;
        }

        public virtual async Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            var posts = await _api.Posts.GetAllBySiteIdAsync<PostInfo>(siteId);

            return posts.Select(x => (Link)x).Where(x => x != null);
        }
    }
}
