using Piranha;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public virtual async Task<IEnumerable<LinkedObject>> GetAllAsync(Guid siteId)
        {
            var pages = await _api.Pages.GetAllAsync<PageInfo>();
            var pagesById = pages.ToDictionary(x => x.Id);

            return pages.Select(x => new LinkedObject(x, GetHierarchicalTitle(x, pagesById))).Where(x => x.Link != null);
        }

        public async Task<LinkedObject> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;

            var page = await _api.Pages.GetByIdAsync<PageInfo>(guid).ConfigureAwait(false);
            if (page != null)
                return new LinkedObject(page) { Content = page };

            // Backward-compat fallback: old data may have stored "Page" for what is actually a post
            var post = await _api.Posts.GetByIdAsync<PostInfo>(guid).ConfigureAwait(false);
            if (post != null)
                return new LinkedObject(post) { Content = post };

            return null;
        }

        private static string GetHierarchicalTitle(PageInfo page, Dictionary<Guid, PageInfo> allPages, string separator = " > ")
        {
            var output = new StringBuilder(GetContentTitle(page));
            var parentId = page.ParentId;
            separator ??= " > ";

            while (parentId.HasValue)
            {
                if (allPages.TryGetValue(parentId.Value, out var parent))
                {
                    parentId = parent.ParentId;
                    output.Insert(0, separator);
                    output.Insert(0, GetContentTitle(parent));
                }
                else
                {
                    parentId = null;
                }
            }

            return output.ToString();
        }

        private static string GetContentTitle(PageInfo page)
        {
            return !string.IsNullOrWhiteSpace(page.NavigationTitle) ? page.NavigationTitle : page.Title;
        }
    }
}
