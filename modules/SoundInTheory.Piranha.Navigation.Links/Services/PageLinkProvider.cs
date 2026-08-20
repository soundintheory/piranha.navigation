using Microsoft.AspNetCore.Http;
using Piranha;
using Piranha.AspNetCore.Services;
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

        private readonly ApplicationServiceAccessor _appAccessor;

        public PageLinkProvider(IApi api, ApplicationServiceAccessor appAccessor)
        {
            _api = api;
            _appAccessor = appAccessor;
        }

        public string LinkType => Models.LinkType.Page;

        public virtual async Task<IEnumerable<LinkedObject>> GetAllAsync(Guid siteId)
        {
            var pages = await _api.Pages.GetAllAsync<PageInfo>(siteId);
            var pagesById = pages.ToDictionary(x => x.Id);

            return pages.Select(x => new LinkedObject(Link.FromPage(x, _appAccessor.ApplicationService), GetHierarchicalTitle(x, pagesById))).Where(x => x.Link != null);
        }

        public async Task<LinkedObject> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;

            var page = await _api.Pages.GetByIdAsync<PageInfo>(guid).ConfigureAwait(false);
            if (page != null)
                return new LinkedObject(Link.FromPage(page, _appAccessor.ApplicationService)) { Content = page };

            // Backward-compat fallback: old data may have stored "Page" for what is actually a post
            var post = await _api.Posts.GetByIdAsync<PostInfo>(guid).ConfigureAwait(false);
            if (post != null)
                return new LinkedObject(Link.FromPost(post, _appAccessor.ApplicationService)) { Content = post };

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
