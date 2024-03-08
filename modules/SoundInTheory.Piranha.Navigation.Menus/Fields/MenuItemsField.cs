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
    [FieldType(Name = "Menu Items Field", Shorthand = "MenuItemsField", Component = "menu-items-field")]
    public class MenuItemsField : SimpleField<List<MenuItem>>
    {
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
    }
}
