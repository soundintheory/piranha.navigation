using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Fields
{
    public interface IHasSerializerSettings
    {
        public JsonSerializerSettings SerializerSettings { get; }
    }
}
