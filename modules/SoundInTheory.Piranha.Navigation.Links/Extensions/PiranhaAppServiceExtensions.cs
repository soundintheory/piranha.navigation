using Microsoft.Extensions.DependencyInjection;
using Piranha.AspNetCore.Services;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Piranha.Manager.Models.PostListModel;

namespace SoundInTheory.Piranha.Navigation.Extensions
{
    public static class PiranhaAppServiceExtensions
    {
        /// <summary>
        /// Generates a url for the given slug including the prefix for the given site
        /// </summary>
        public static string UrlWithPrefix(this IApplicationService app, string link, Guid? siteId = null)
        {
            var prefix = GetSitePrefix(app, siteId);
            return $"{prefix}{link}";
        }

        /// <summary>
        /// Generates a local url for the given page including the prefix for its owning site
        /// </summary>
        public static string UrlWithPrefix(this IApplicationService app, PageBase page)
        {
            if (page != null)
            {
                return UrlWithPrefix(app, page.Permalink, page.SiteId);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates a local url for the given page field including the prefix for its owning site
        /// </summary>
        public static string UrlWithPrefix(this IApplicationService app, PageField field)
        {
            if (field != null)
            {
                return UrlWithPrefix(app, field.Page);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates a local url for the given page block including the prefix for its owning site
        /// </summary>
        public static string UrlWithPrefix(this IApplicationService app, PageBlock block)
        {
            if (block != null)
            {
                return UrlWithPrefix(app, block.Body);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates a local url for the given post including the prefix for its owning site
        /// </summary>
        public static string UrlWithPrefix(this IApplicationService app, PostBase post)
        {
            if (post != null)
            {
                var archive = app.Api.Pages.GetByIdAsync<PageInfo>(post.BlogId).GetAwaiter().GetResult();

                if (archive != null)
                {
                    return UrlWithPrefix(app, post.Permalink, archive.SiteId);
                }

                return post.Permalink;
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates a local url for the given post field including the prefix for its owning site
        /// </summary>
        public static string UrlWithPrefix(this IApplicationService app, PostField field)
        {
            if (field != null)
            {
                return UrlWithPrefix(app, field.Post);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates a local url for the given post block including site prefix for its owning site
        /// </summary>
        public static string UrlWithPrefix(this IApplicationService app, PostBlock block)
        {
            if (block != null)
            {
                return UrlWithPrefix(app, block.Body);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates an absolute url for the given link including the prefix for the given site
        /// </summary>
        public static string AbsoluteUrlWithPrefix(this IApplicationService app, string link, Guid? siteId = null)
        {
            var url = UrlWithPrefix(app, link, siteId);

            return IsAbsoluteUrl(url) ? url : $"{AbsoluteUrlStart(app)}{url}";
        }

        /// <summary>
        /// Generates an absolute url for the given page including the prefix for its owning site
        /// </summary>
        public static string AbsoluteUrlWithPrefix(this IApplicationService app, PageBase page)
        {
            var url = UrlWithPrefix(app, page);

            return IsAbsoluteUrl(url) ? url : $"{AbsoluteUrlStart(app)}{url}";
        }

        /// <summary>
        /// Generates an absolute url for the given page field including the prefix for its owning site
        /// </summary>
        public static string AbsoluteUrlWithPrefix(this IApplicationService app, PageField field)
        {
            if (field != null)
            {
                return AbsoluteUrlWithPrefix(app, field.Page);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates an absolute url for the given page block including the prefix for its owning site
        /// </summary>
        public static string AbsoluteUrlWithPrefix(this IApplicationService app, PageBlock block)
        {
            if (block != null)
            {
                return AbsoluteUrlWithPrefix(app, block.Body);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates an absolute url for the given post including the prefix for its owning site
        /// </summary>
        public static string AbsoluteUrlWithPrefix(this IApplicationService app, PostBase post)
        {
            var url = UrlWithPrefix(app, post);

            return IsAbsoluteUrl(url) ? url : $"{AbsoluteUrlStart(app)}{url}";
        }

        /// <summary>
        /// Generates an absolute url for the given post field including the prefix for its owning site
        /// </summary>
        public static string AbsoluteUrlWithPrefix(this IApplicationService app, PostField field)
        {
            if (field != null)
            {
                return AbsoluteUrlWithPrefix(app, field.Post);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates an absolute url for the given post block including the prefix for its owning site
        /// </summary>
        public static string AbsoluteUrlWithPrefix(this IApplicationService app, PostBlock block)
        {
            if (block != null)
            {
                return AbsoluteUrlWithPrefix(app, block.Body);
            }
            return string.Empty;
        }

        private static ConcurrentDictionary<string, string> _sitePrefixCache = new();

        private static bool? _allSitesHaveSameHost;

        /// <summary>
        /// Gets the url prefix for the given site. Can be absolute or relative depending on site configuration
        /// </summary>
        public static string GetSitePrefix(this IApplicationService app, Guid? siteId)
        {
            var cacheKey = $"{app.Request.Host ?? string.Empty}_{siteId ?? Guid.Empty}";

            return _sitePrefixCache.GetOrAdd(cacheKey, key =>
            {
                var site = siteId.HasValue ? app.Api.Sites.GetByIdAsync(siteId.Value).GetAwaiter().GetResult() : app.Api.Sites.GetDefaultAsync().GetAwaiter().GetResult();

                if (site == null || site.IsDefault)
                {
                    return string.Empty;
                }

                var siteHost = GetFirstHost(site);

                if (AllSitesHaveSameHost(app) || siteHost[0] == app.Request.Host)
                {
                    return $"/{siteHost[1]}";
                }

                var scheme = app.Request.Scheme ?? "https";
                var port = app.Request.Port != null ? $":{app.Request.Port}" : string.Empty;

                return $"{scheme}://{siteHost[0]}{port}/{siteHost[1]}";
            });
        }

        internal static bool AllSitesHaveSameHost(this IApplicationService app)
        {
            if (!_allSitesHaveSameHost.HasValue)
            {
                var hostCount = app.Api.Sites.GetAllAsync().GetAwaiter().GetResult().Select(site => GetFirstHost(site)[0]).Distinct().Count();
                _allSitesHaveSameHost = hostCount == 1;
            }
            return _allSitesHaveSameHost.Value;
        }

        internal static void ClearCache()
        {
            _sitePrefixCache.Clear();
            _allSitesHaveSameHost = null;
        }

        private static bool IsAbsoluteUrl(ReadOnlySpan<char> url)
        {
            return url.StartsWith("http") || url.StartsWith("//");
        }

        /// <summary>
        /// Generates the scheme://host:port segment of the url from
        /// the current application request.
        /// </summary>
        private static string AbsoluteUrlStart(IApplicationService app)
        {
            var sb = new StringBuilder();

            sb.Append(app.Request.Scheme);
            sb.Append("://");
            sb.Append(app.Request.Host);
            if (app.Request.Port.HasValue)
            {
                sb.Append(":");
                sb.Append(app.Request.Port.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the first hostname of the site.
        /// </summary>
        /// <param name="site">The site</param>
        /// <returns>The hostname split into host and prefix</returns>
        private static string[] GetFirstHost(Site site)
        {
            var result = new string[2] { string.Empty, string.Empty };

            if (!string.IsNullOrEmpty(site.Hostnames))
            {
                var hostname = site.Hostnames.Split(",").FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                if (hostname != null)
                {
                    var segments = hostname.Split("/", StringSplitOptions.RemoveEmptyEntries);

                    result[0] = segments[0];
                    result[1] = segments.Length > 1 ? segments[1] : string.Empty;

                }
            }
            return result;
        }
    }
}
