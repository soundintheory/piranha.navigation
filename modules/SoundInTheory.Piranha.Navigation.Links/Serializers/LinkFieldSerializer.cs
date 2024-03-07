using Newtonsoft.Json;
using Piranha.Extend;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Serializers
{
    public class LinkFieldSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            if (obj is LinkField field)
            {
                // Don't store null attributes
                if (field.Attributes != null)
                {
                    var nonEmptyAttributes = field.Attributes.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
                    field.Attributes = nonEmptyAttributes.Any() ? nonEmptyAttributes : null;
                }

                // Never use content title for child links
                if (!string.IsNullOrEmpty(field.Path))
                {
                    field.UseContentTitle = false;
                }
                
                return JsonConvert.SerializeObject(field, _settings);
            }
            throw new ArgumentException("The given object doesn't match the serialization type");
        }

        public object Deserialize(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new LinkField();
            }

            try
            {
                return JsonConvert.DeserializeObject<LinkField>(str);
            }
            catch (JsonReaderException)
            {
                if (Guid.TryParse(str, out var id))
                {
                    return new LinkField { Id = id, Type = LinkType.Page, UseContentTitle = true };
                }

                if (!string.IsNullOrWhiteSpace(str))
                {
                    return new LinkField { Type = LinkType.Custom, Url = str.Trim(), Text = str.Trim() };
                }
            }

            return new LinkField();
        }

        private readonly JsonSerializerSettings _settings = new() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
    }
}
