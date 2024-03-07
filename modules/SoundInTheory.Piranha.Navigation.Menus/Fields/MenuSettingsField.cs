using Piranha.Extend;
using Piranha.Extend.Fields;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation
{
    [FieldType(Name = "Menu Settings Field", Shorthand = "MenuSettingsField", Component = "menu-settings-field")]
    public class MenuSettingsField : SimpleField<MenuSettings>
    {
        /// <summary>
        /// Implicit operator for converting a string to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator MenuSettingsField(MenuSettings obj)
        {
            return new MenuSettingsField { Value = obj };
        }

        /// <summary>
        /// Implicitly converts the String field to a string.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator MenuSettings(MenuSettingsField field)
        {
            return field != null ? field.Value : default;
        }
    }
}
