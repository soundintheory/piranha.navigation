using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Piranha;
using Piranha.Extend;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Web.Sitemap;

namespace SoundInTheory.Piranha.Navigation.Fields
{
    [FieldType(Name = "MenuLinkField", Component = "menu-link-field")]
    public class MenuLinkField : LinkField, IEquatable<MenuLinkField>
    {
        public MenuLinkField() { }

        public MenuLinkField(ILink link) : base(link) { }

        /// <summary>
        /// Implicit operator for converting a Piranha content object to a link tag
        /// </summary>
        /// <param name="content">The content object</param>
        public static implicit operator MenuLinkField(RoutedContentBase content)
        {
            return new MenuLinkField
            {
                Id = content.Id,
                Url = content.Permalink,
                Text = content.Title,
                Type = content is PostBase ? LinkType.Post : LinkType.Page,
            };
        }

        /// <summary>
        /// Checks if the given field is equal to the field.
        /// </summary>
        /// <param name="obj">The field</param>
        /// <returns>True if the fields are equal</returns>
        public virtual bool Equals(MenuLinkField obj) => base.Equals(obj);

        /// <summary>
        /// Checks if the fields are equal.
        /// </summary>
        /// <param name="link1">The first field</param>
        /// <param name="link2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator ==(MenuLinkField link1, MenuLinkField link2)
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
        public static bool operator !=(MenuLinkField link1, MenuLinkField link2)
        {
            return !(link1 == link2);
        }
    }
}
