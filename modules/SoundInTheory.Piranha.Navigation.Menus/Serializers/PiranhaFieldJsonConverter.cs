using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Piranha;
using Piranha.Extend;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Serializers
{
    public class PiranhaFieldJsonConverter : JsonConverter<IField>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, IField value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // Use Piranha serializer
            var serialized = App.SerializeObject(value, value.GetType());

            writer.WriteValue(serialized);
        }

        public override IField ReadJson(JsonReader reader, Type objectType, IField existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var field = JToken.Load(reader);
            var fieldContent = field.ToString();

            return (IField)App.DeserializeObject(fieldContent, objectType);
        }
    }
}
