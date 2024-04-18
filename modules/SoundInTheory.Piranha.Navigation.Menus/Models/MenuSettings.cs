using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SoundInTheory.Piranha.Navigation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public class MenuSettings
    {
        public int MaxDepth { get; set; }


        public List<EditorMenuOption> EnabledOptions { get; set; } 
    }
}
