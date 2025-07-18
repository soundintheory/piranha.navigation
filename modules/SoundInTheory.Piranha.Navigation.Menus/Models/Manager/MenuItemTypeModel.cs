using Microsoft.VisualBasic.FileIO;
using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Manager.Models.Content;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Models.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models.Manager
{
    public class MenuItemTypeModel
    {
        /// <summary>
        /// Gets/sets the unique id for this menu item type.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the display name for this menu item type.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the CLR type for this menu item.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets/sets an optional description for this menu item type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Which types of menu items can be children of this. An empty array means any type is allowed
        /// </summary>
        public string[] AllowedChildren { get; set; }

        /// <summary>
        /// Which types of menu items can be parents of this. An empty array means any type is allowed
        /// </summary>
        public string[] AllowedParents { get; set; }

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
        /// Gets/sets the fields available on this menu item type.
        /// </summary>
        public IList<FieldModel> Fields { get; set; } = new List<FieldModel>();
    }
}
