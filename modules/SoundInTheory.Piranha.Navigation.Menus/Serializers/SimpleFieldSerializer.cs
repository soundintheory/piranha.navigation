using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Piranha.Extend;
using Piranha.Extend.Fields;
using SoundInTheory.Piranha.Navigation.Fields;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Serializers
{
    /// <summary>
    /// Serialises and deserialises the 'Value' property for instances of SimpleField
    /// </summary>
    /// <typeparam name="TField">The field type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public class SimpleFieldSerializer<TField, TValue> : ISerializer where TField : SimpleField<TValue>, new() where TValue : new()
    {
        /// <inheritdoc />
        public string Serialize(object obj)
        {
            if (obj is TField field)
            {
                if (field is IHasSerializerSettings sField)
                {
                    return JsonConvert.SerializeObject(field.Value, sField.SerializerSettings);
                }

                return JsonConvert.SerializeObject(field.Value);
            }
            throw new ArgumentException("The given object doesn't match the serialization type");
        }

        /// <inheritdoc />
        public object Deserialize(string str)
        {
            try
            {
                var field = new TField();

                if (field is IHasSerializerSettings sField)
                {
                    field.Value = JsonConvert.DeserializeObject<TValue>(str, sField.SerializerSettings);
                }
                else
                {
                    field.Value = JsonConvert.DeserializeObject<TValue>(str);
                }

                return field;
            }
            catch (Exception)
            {
                return new TField { Value = new TValue() };
            }
        }
    }
}
