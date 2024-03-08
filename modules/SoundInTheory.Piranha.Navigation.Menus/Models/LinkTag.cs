using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Piranha;
using Piranha.Models;
using X.Web.Sitemap;
using Microsoft.CodeAnalysis;
using SoundInTheory.Piranha.Navigation.Models;

namespace SoundInTheory.Piranha.Navigation.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class LinkTag : ILink, IEquatable<LinkTag>
    {
        public LinkTag() { }

        public LinkTag(ILink link)
        {
            if (link != null)
            {
                Type = link.Type;
                Url = link.Url;
                Text = link.Text;
                Id = link.Id;
                TypeId = link.TypeId;

                if (link is LinkTag linkTag)
                {
                    Attributes = linkTag.Attributes.ToDictionary(x => x.Key, x => x.Value);
                    UseContentTitle = linkTag.UseContentTitle;
                    ContentLink = new Link(linkTag.ContentLink);
                }
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public LinkType Type { get; set; }

        public string Url { get; set; }

        public string Path { get; set; }

        public string Text { get; set; }

        public Guid? Id { get; set; }

        public Guid? ParentId { get; set; }

        [JsonIgnore]
        public string TypeId { get; set; }

        /// <summary>
        /// Custom HTML attributes for the link when rendered
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Whether the text should be the title of the content being linked to (only for page / post links)
        /// </summary>
        public bool UseContentTitle { get; set; }

        /// <summary>
        /// Holds a reference to the content being linked to
        /// </summary>
        public Link ContentLink { get; set; }

        /// <summary>
        /// Initializes the link after being loaded from the database. If it is a page or post link then
        /// the url (and text if applicable) will be updated to reflect it
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual async Task Init(IApi api)
        {
            var content = await GetContentInfo(api).ConfigureAwait(false);

            if (content != null)
            {
                Url = content.Permalink;
                ContentLink = new Link
                {
                    Type = Type,
                    Url = content.Permalink,
                    Text = content.Title,
                    Id = Id,
                    TypeId = content.TypeId
                };

                if (UseContentTitle)
                {
                    Text = content.Title;
                }
            } 
            else
            {
                ContentLink = null;
            }
        }

        /// <summary>
        /// Implicit operator for converting a Piranha content object to a link tag
        /// </summary>
        /// <param name="content">The content object</param>
        public static implicit operator LinkTag(RoutedContentBase content)
        {
            return new LinkTag
            {
                Id = content.Id,
                Url = content.Permalink,
                Text = content.Title,
                Type = content is PostBase ? LinkType.Post : LinkType.Page,
            };
        }

        /// <summary>
        /// Gets the hash code for the field.
        /// </summary>
        public override int GetHashCode()
        {
            switch (Type)
            {
                case LinkType.Page:
                case LinkType.Post:
                    return Id.HasValue ? Id.GetHashCode() : 0;
            }

            return !string.IsNullOrWhiteSpace(Url) ? Url.GetHashCode() : 0;
        }

        /// <summary>
        /// Checks if the given object is equal to the field.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>True if the fields are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is LinkTag link)
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
        public virtual bool Equals(LinkTag obj)
        {
            if (obj is null || obj.Type != Type)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            switch (Type)
            {
                case LinkType.Page:
                case LinkType.Post:
                    return Id == obj.Id;
            }

            return Url == obj.Url;
        }

        /// <summary>
        /// Checks if the fields are equal.
        /// </summary>
        /// <param name="link1">The first field</param>
        /// <param name="link2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator ==(LinkTag link1, LinkTag link2)
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
        public static bool operator !=(LinkTag link1, LinkTag link2)
        {
            return !(link1 == link2);
        }

        /// <summary>
        /// Gets the referenced page or post. Will return null if the type is not page or post
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The referenced page or post</returns>
        protected async Task<RoutedContentBase> GetContentInfo(IApi api)
        {
            if (Id.HasValue)
            {
                switch (Type)
                {
                    case LinkType.Page:
                        return await api.Pages
                            .GetByIdAsync<PageInfo>(Id.Value)
                            .ConfigureAwait(false);
                    case LinkType.Post:
                        return await api.Posts
                            .GetByIdAsync<PostInfo>(Id.Value)
                            .ConfigureAwait(false);
                }
            }

            return null;
        }
    }
}
