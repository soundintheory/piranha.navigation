using Piranha.Manager.Services;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public class PostLinkProvider : ILinkProvider
    {
        private readonly PostService _postService;

        public PostLinkProvider(PostService postService)
        {
            _postService = postService;
        }

        public virtual async Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            var archiveMap = await _postService.GetArchiveMap(siteId, null);

            return archiveMap.Posts.Select(x => (Link)x);
        }
    }
}
