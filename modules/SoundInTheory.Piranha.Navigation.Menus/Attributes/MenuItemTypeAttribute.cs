using Piranha.AttributeBuilder;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuItemTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the unique id for this menu item type. Defaults to the class name
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the title for this menu item type.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets an optional description for this menu item type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Which types of menu items can be children of this. An empty array means any type is allowed
        /// </summary>
        public Type[] AllowedChildren { get; set; }

        /// <summary>
        /// Which types of menu items can be parents of this. An empty array means any type is allowed
        /// </summary>
        public Type[] AllowedParents { get; set; }

        /// <summary>
        /// The maximum level this item type can be at. Zero means any level is allowed
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Maximum depth of items within this item. Overrides the menu MaxDepth if set
        /// </summary>
        public int MaxDepth { get; set; } = -1;

        /// <summary>
        /// The property to use as the list title
        /// </summary>
        public string ListTitle { get; set; }

        /// <summary>
        /// The property to use as the list type
        /// </summary>
        public string ListType { get; set; }

        /// <summary>
        /// The path to a view to use when rendering
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// The class name to use for list items of this type
        /// </summary>
        public string CssClass { get; set; }
    }
}
