using Newtonsoft.Json;
using Piranha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public class MenuItem : ITreeNode<MenuItem>
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
	    public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the JSON serialized link value.
        /// </summary>
        public LinkTag Link { get; set; }

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
        public IList<MenuItem> Children { get; set; }

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
    }
}
