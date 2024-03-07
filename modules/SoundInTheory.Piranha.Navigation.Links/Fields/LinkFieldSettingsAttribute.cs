using Piranha.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation
{
    /// <summary>
    /// Field settings for link field
    /// </summary>
    public class LinkFieldSettingsAttribute : FieldSettingsAttribute
    {
        public bool HideLinkText { get; set; }
    }
}
