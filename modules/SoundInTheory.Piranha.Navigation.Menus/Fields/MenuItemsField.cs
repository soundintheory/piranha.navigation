using Newtonsoft.Json;
using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Extensions;
using SoundInTheory.Piranha.Navigation.Fields;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Web.Sitemap;
using static System.Net.Mime.MediaTypeNames;

namespace SoundInTheory.Piranha.Navigation
{
    [FieldType(Name = "Menu Items Field", Shorthand = "MenuItemsField", Component = "menu-items-field")]
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.None)]
    public class MenuItemsField : SimpleField<List<MenuItem>>, IHasSerializerSettings
    {
        public JsonSerializerSettings SerializerSettings => new()
        {
            Converters = { new PiranhaFieldJsonConverter() },
            TypeNameHandling = TypeNameHandling.None
        };

        public override string GetTitle() => "";

        /// <summary>
        /// Implicit operator for converting a string to a field.
        /// </summary>
        /// <param name="items">The menu item list</param>
        public static implicit operator MenuItemsField(List<MenuItem> items)
        {
            return new MenuItemsField { Value = items };
        }

        /// <summary>
        /// Implicitly converts the String field to a string.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator List<MenuItem>(MenuItemsField field)
        {
            return field?.Value ?? new List<MenuItem>();
        }

        public virtual async Task Init(IApi api)
        {
            Value?.ForEachRecursive((item, level) => item.Level = level);
        }
    }
}
