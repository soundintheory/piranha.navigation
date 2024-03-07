using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Extensions;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation
{
    [FieldType(Name = "LinkField", Shorthand = "LinkField", Component = "link-field")]
    public class LinkField : Field, ILink, IEquatable<LinkField>
    {
        /// <summary>
        /// The page or post id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// The parent item id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// The type of link - can be page, post, or custom
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LinkType Type { get; set; } = LinkType.Custom;

        /// <summary>
        /// The url of the link
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The path of the link
        /// </summary>
        public string Path { get; set; }

        // <summary>
        /// The readable text of the link
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Any additional HTML attributes to add to the link
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Whether the text should be the title of the content being linked to (only for page / post links)
        /// </summary>
        public bool UseContentTitle { get; set; }

        /// <summary>
        /// Information about the Piranha content being linked to, if the type of link is page or post
        /// </summary>
        public Link ContentLink { get; internal set; }

        [JsonIgnore]
        public RoutedContentBase ContentInfo { get; internal set; }

        /// <summary>
        /// The type ID of the content link
        /// </summary>
        [JsonIgnore]
        public string TypeId
        {
            get => ContentInfo?.TypeId;
            set {
                // Setting the TypeId will have no effect here
            }
        }

        public bool HasValue => !this.IsNullOrEmpty();

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection
        /// </summary>
        public override string GetTitle() => Text;

        public virtual async Task Init(IApi api)
        {
            ContentInfo = await GetContentInfo(api);
            ContentLink = (Link)ContentInfo;
            
            if (ContentLink != null)
            {
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    ContentLink = ContentLink.SubLink(Path);
                }

                Type = ContentLink.Type;
                Url = ContentLink.Url.AppendUrlPath(Path);

                if (UseContentTitle || Text == null)
                {
                    Text = ContentLink.Text;
                }
            }
        }

        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The guid value</param>
        public static implicit operator LinkField(Guid guid)
        {
            return new LinkField { Id = guid, Type = LinkType.Page };
        }

        /// <summary>
        /// Implicit operator for converting a Piranha content object to a field
        /// </summary>
        /// <param name="page">The content object</param>
        public static implicit operator LinkField(RoutedContentBase content)
        {
            return new LinkField { Id = content.Id };
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
            if (obj is LinkField field)
            {
                return Equals(field);
            }
            return false;
        }

        /// <summary>
        /// Checks if the given field is equal to the field.
        /// </summary>
        /// <param name="obj">The field</param>
        /// <returns>True if the fields are equal</returns>
        public virtual bool Equals(LinkField obj)
        {
            if (obj == null || obj.Type != Type)
            {
                return false;
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
        /// <param name="field1">The first field</param>
        /// <param name="field2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator ==(LinkField field1, LinkField field2)
        {
            if (field1 is not null && field2 is not null)
            {
                return field1.Equals(field2);
            }

            return field1 is null && field2 is null;
        }

        /// <summary>
        /// Checks if the fields are not equal.
        /// </summary>
        /// <param name="field1">The first field</param>
        /// <param name="field2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator !=(LinkField field1, LinkField field2)
        {
            return !(field1 == field2);
        }

        /// <summary>
        /// Gets the referenced page or post. Will return null if the type is not page or post
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The referenced page or post</returns>
        private async Task<RoutedContentBase> GetContentInfo(IApi api)
        {
            if (Id.HasValue)
            {
                switch (Type)
                {
                    case LinkType.Page:
                        return (await GetPage(api)) ?? (await GetPost(api));
                    case LinkType.Post:
                        return (await GetPost(api)) ?? (await GetPage(api));
                }
            }

            return null;
        }

        private async Task<RoutedContentBase> GetPage(IApi api)
        {
            return await api.Pages
                .GetByIdAsync<PageInfo>(Id.Value)
                .ConfigureAwait(false);
        }

        private async Task<RoutedContentBase> GetPost(IApi api)
        {
            return await api.Posts
                .GetByIdAsync<PostInfo>(Id.Value)
                .ConfigureAwait(false);
        }
    }
}
