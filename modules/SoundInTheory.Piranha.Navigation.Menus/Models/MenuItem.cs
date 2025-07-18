using Newtonsoft.Json;
using Piranha;
using SoundInTheory.Piranha.Navigation.Rendering;
using SoundInTheory.Piranha.Navigation.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.None), JsonArray(ItemTypeNameHandling = TypeNameHandling.None), JsonConverter(typeof(MenuItemJsonConverter))]
    public abstract class MenuItem : ITreeNode<MenuItem>
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
	    public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
	    public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
	    public DateTime LastModified { get; set; }

        /// <summary>
        /// Contains the items children
        /// </summary>
        public IList<MenuItem> Children { get; set; } = new List<MenuItem>();

        /// <summary>
        /// The items parent
        /// </summary>
        [JsonIgnore]
        public MenuItem Parent { get; set; }

        /// <summary>
        /// Whether the item is active
        /// </summary>
        [JsonIgnore]
        public bool Active { get; set; }

        /// <summary>
        /// Whether the item should be hidden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Whether the item contains an active item
        /// </summary>
        [JsonIgnore]
        public bool ParentActive { get; set; }

        /// <summary>
        /// The level of this item in the tree
        /// </summary>
        [JsonIgnore]
        public int Level { get; set; }

        /// <summary>
        /// Discriminator property describing the menu item type id
        /// </summary>
        [JsonProperty(PropertyName = "$typeId")]
        public string _TypeId => MenuModule.Instance.MenuItems.GetByType(this.GetType())?.Id ?? "Link";

        /// <summary>
        /// Called when the item is loaded
        /// </summary>
        public virtual async Task Init(IApi api)
        {
        }

        /// <summary>
        /// Renders the menu item to the output in the provided context
        /// </summary>
        /// <param name="viewModel">The render context containing output and options</param>
        public virtual void Render(MenuItemViewModel viewModel)
        {
            // Default implementation - renders nothing
            // Concrete menu item types should override this method
        }

        /// <summary>
        /// Checks whether the item is active in the current context
        /// </summary>
        /// <param name="viewModel">The render context containing output and options</param>
        public virtual bool IsActive(MenuItemViewModel viewModel)
        {
            return false;
        }

        /// <summary>
        /// Used by the renderer to check whether the item can be rendered
        /// </summary>
        [JsonIgnore]
        public virtual bool IsValid => true;
    }
}
