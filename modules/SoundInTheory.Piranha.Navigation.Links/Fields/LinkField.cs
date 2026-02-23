using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Piranha.Extend;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Extensions;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation
{
    [FieldType(Name = "LinkField", Shorthand = "LinkField", Component = "link-field")]
    public class LinkField : Field, ILink, IEquatable<LinkField>
    {
        public LinkField() { }

        public LinkField(ILink link)
        {
            if (link != null)
            {
                Type = link.Type;
                Url = link.Url;
                Text = link.Text;
                Id = link.Id;
                TypeId = link.TypeId;

                if (link is LinkField linkField)
                {
                    Attributes = linkField.Attributes.ToDictionary(x => x.Key, x => x.Value);
                    UseContentTitle = linkField.UseContentTitle;
                    ContentLink = new Link(linkField.ContentLink);
                }
            }
        }

        /// <summary>
        /// The page or post id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The type of link - can be page, post, custom, or a custom provider's type string
        /// </summary>
        public string Type { get; set; } = LinkType.Custom;

        /// <summary>
        /// The url of the link
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The path of the link
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The readable text of the link
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Any additional HTML attributes to add to the link
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Whether the text should be the title of the content being linked to (if the link has come from a provider)
        /// </summary>
        public bool UseContentTitle { get; set; }

        /// <summary>
        /// Information about the content being linked to, if the link has come from a provider (eg. page or post)
        /// </summary>
        public Link ContentLink { get; protected set; }

        [JsonIgnore]
        public RoutedContentBase ContentInfo { get; protected set; }

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

        [JsonIgnore]
        public bool HasValue => !this.IsNullOrEmpty();

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection
        /// </summary>
        public override string GetTitle() => Text;

        public virtual async Task Init(IServiceProvider services)
        {
            if (string.IsNullOrEmpty(Id) || Type == LinkType.None || Type == LinkType.Custom)
                return;

            var linkService = services.GetRequiredService<LinkService>();
            var result = await linkService.GetByIdAsync(Id, Type).ConfigureAwait(false);

            if (result != null)
                ApplyResolution(result.Link, result.Content as RoutedContentBase);
        }

        private void ApplyResolution(Link contentLink, RoutedContentBase contentInfo)
        {
            ContentInfo = contentInfo;
            ContentLink = contentLink;

            if (ContentLink == null) return;

            if (!string.IsNullOrWhiteSpace(Path))
                ContentLink = ContentLink.SubLink(Path);

            Type = ContentLink.Type;
            Url = ContentLink.Url.AppendUrlPath(Path);

            if (UseContentTitle || Text == null)
                Text = ContentLink.Text;
        }

        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The guid value</param>
        public static implicit operator LinkField(Guid guid)
        {
            return new LinkField { Id = guid.ToString(), Type = LinkType.Page, UseContentTitle = true };
        }

        /// <summary>
        /// Implicit operator for converting a Piranha content object to a field
        /// </summary>
        /// <param name="content">The content object</param>
        public static implicit operator LinkField(RoutedContentBase content)
        {
            return new LinkField { Id = content.Id.ToString(), Type = (content is PostBase) ? LinkType.Post : LinkType.Page, UseContentTitle = true };
        }

        /// <summary>
        /// Gets the hash code for the field.
        /// </summary>
        public override int GetHashCode()
        {
            if (Id != null)
                return HashCode.Combine(Id, Type, Path, UseContentTitle, UseContentTitle ? "" : Text);

            return HashCode.Combine(Type, Url, Path, Text);
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

            if (Type == LinkType.None)
                return Text == obj.Text;

            if (Type == LinkType.Custom)
                return Url == obj.Url && Path == obj.Path && Text == obj.Text;

            return Id == obj.Id && Path == obj.Path && ((UseContentTitle && obj.UseContentTitle) || (!UseContentTitle && !obj.UseContentTitle && Text == obj.Text));
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
    }
}
