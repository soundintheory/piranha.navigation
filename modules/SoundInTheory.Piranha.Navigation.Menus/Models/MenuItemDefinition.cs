using Piranha.Models;
using System;
using System.Collections.Generic;

namespace SoundInTheory.Piranha.Navigation.Models
{
    /// <summary>
    /// Definition for a menu item type that can be registered and used in menus.
    /// </summary>
    public class MenuItemDefinition
    {
        /// <summary>
        /// Gets/sets the unique id for this menu item type. Defaults to the class name
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the display name for this menu item type.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the CLR type for this menu item.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets/sets an optional description for this menu item type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets the fields available on this menu item type.
        /// </summary>
        public IList<ContentTypeField> Fields { get; set; } = new List<ContentTypeField>();

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
        public int? MaxDepth { get; set; }

        /// <summary>
        /// The property to use as the list title
        /// </summary>
        public string ListTitle { get; set; }

        /// <summary>
        /// The property to use as the list type
        /// </summary>
        public string ListType { get; set; }

        /// <summary>
        /// The view name to use when rendering
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// The class name to use for list items of this type
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets the type name used for JSON serialization discrimination.
        /// </summary>
        public string TypeName => Type?.FullName;
    }
}
