using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Piranha;
using Piranha.Extend;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoundInTheory.Piranha.Navigation.Serializers
{
    /// <summary>
    /// JSON converter for polymorphic MenuItem serialization/deserialization using "$type" discriminator.
    /// </summary>
    public class MenuItemJsonConverter : JsonConverter<MenuItem>
    {
        const string DISCRIMINATOR = "$typeId";

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, MenuItem value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override MenuItem ReadJson(JsonReader reader, Type objectType, MenuItem existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jo = JObject.Load(reader);

            // Get the target type from the menu item definition. Fall back to LinkMenuitem for backwards compatibility
            var targetType = MenuModule.Instance.MenuItems.GetById(jo[DISCRIMINATOR]?.Value<string>())?.Type ?? typeof(LinkMenuItem);

            MenuItem item = (MenuItem)Activator.CreateInstance(targetType);
            serializer.Populate(jo.CreateReader(), item);
            item.Children ??= new List<MenuItem>();

            return item;
        }
    }
}
