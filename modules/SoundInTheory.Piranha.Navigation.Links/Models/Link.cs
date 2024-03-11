using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Piranha.Models;
using Piranha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Piranha.Manager.Models.PageListModel;
using static Piranha.Manager.Models.PostModalModel;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using SoundInTheory.Piranha.Navigation.Extensions;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public class Link : ILink, IEquatable<Link>
    {
        public Link() { }

        public Link(ILink link)
        {
            if (link != null)
            {
                Type = link.Type;
                Url = link.Url;
                Text = link.Text;
                Id = link.Id;
                TypeId = link.TypeId;
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public LinkType Type { get; set; }

        public string Url { get; set; }

        public string Path { get; protected set; }

        public string Text { get; set; }

        public Guid? Id { get; set; }
        
        public Dictionary<string, object> Attributes { get; set; }

        protected string UrlExcludingPath => Url.RemoveUrlPath(Path);

        public Link SubLink(string path, string title = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Cannot create sub link with null or empty path");
            }

            if (string.IsNullOrWhiteSpace(Path))
            {
                var newPath = path.Trim('/');

                return new Link
                {
                    Type = Type,
                    Id = Id,
                    Url = Url.TrimEnd('/') + "/" + newPath,
                    Path = newPath,
                    Text = !string.IsNullOrWhiteSpace(title) ? $"{Text} > {title}" : $"{Text} > {path}"
                };
            }
            else
            {
                var newPath = Path.AppendUrlPath(path).Trim('/');

                return new Link
                {
                    Type = Type,
                    Id = Id,
                    Url = UrlExcludingPath + "/" + newPath,
                    Path = newPath,
                    Text = !string.IsNullOrWhiteSpace(title) ? $"{Text} > {title}" : $"{Text} > {path}"
                };
            }
        }

        /// <summary>
        /// Implicit operator for converting a Piranha manager page item to a link
        /// </summary>
        /// <param name="pageItem">The content object</param>
        public static implicit operator Link(PageItem pageItem)
        {
            if (pageItem == null)
            {
                return null;
            }

            return new Link
            {
                Id = pageItem.Id,
                Url = pageItem.Permalink,
                Text = pageItem.Title,
                Type = LinkType.Page
            };
        }

        /// <summary>
        /// Implicit operator for converting a Piranha manager post item to a link
        /// </summary>
        /// <param name="postItem">The post object</param>
        public static implicit operator Link(PostModalItem postItem)
        {
            if (postItem == null)
            {
                return null;
            }

            return new Link
            {
                Id = postItem.Id,
                Url = postItem.Permalink,
                Text = postItem.Title,
                Type = LinkType.Post
            };
        }

        [JsonIgnore]
        public string TypeId { get; set; }

        /// <summary>
        /// Implicit operator for converting a Piranha content object to a link
        /// </summary>
        /// <param name="content">The content object</param>
        public static implicit operator Link(RoutedContentBase content)
        {
            if (content == null)
            {
                return null;
            }

            return new Link
            {
                Id = content.Id,
                Url = content.Permalink,
                Text = GetContentTitle(content) ?? content.Slug,
                Type = content is PostBase ? LinkType.Post : LinkType.Page,
                TypeId = content.TypeId
            };
        }

        private static string GetContentTitle(RoutedContentBase content)
        {
            if (content is PageBase page)
            {
                return !string.IsNullOrWhiteSpace(page.NavigationTitle) ? page.NavigationTitle : page.Title;
            }
            return content.Title;
        }

        /// <summary>
        /// Helper method for creating a link object from Piranha content
        /// </summary>
        public static Link FromContent(RoutedContentBase contentItem) => (Link)contentItem;

        /// <summary>
        /// Helper method for creating a link object from a Piranha manager page item
        /// </summary>
        public static Link FromPageItem(PageItem pageItem) => (Link)pageItem;

        /// <summary>
        /// Helper method for creating a link object from a Piranha manager post item
        /// </summary>
        public static Link FromPostItem(PostModalItem postItem) => (Link)postItem;

        /// <summary>
        /// Gets the hash code for the field.
        /// </summary>
        public override int GetHashCode()
        {
            switch (Type)
            {
                case LinkType.Page:
                case LinkType.Post:
                    return HashCode.Combine(Type, Id, Path);
            }

            return HashCode.Combine(Url, Path);
        }

        /// <summary>
        /// Checks if the given object is equal to the field.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>True if the fields are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Link link)
            {
                return Equals(link);
            }
            return false;
        }

        /// <summary>
        /// Checks if the given field is equal to the field.
        /// </summary>
        /// <param name="obj">The field</param>
        /// <returns>True if the fields are equal</returns>
        public virtual bool Equals(Link obj)
        {
            if (obj == null || obj.Type != Type)
            {
                return false;
            }

            switch (Type)
            {
                case LinkType.Page:
                case LinkType.Post:
                    return Id == obj.Id && Path == obj.Path;
            }

            return Url == obj.Url && Path == obj.Path;
        }

        /// <summary>
        /// Checks if the fields are equal.
        /// </summary>
        /// <param name="link1">The first field</param>
        /// <param name="link2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator ==(Link link1, Link link2)
        {
            if (link1 is not null && link2 is not null)
            {
                return link1.Equals(link2);
            }

            return link1 is null && link2 is null;
        }

        /// <summary>
        /// Checks if the fields are not equal.
        /// </summary>
        /// <param name="link1">The first field</param>
        /// <param name="link2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator !=(Link link1, Link link2)
        {
            return !(link1 == link2);
        }
    }
}
