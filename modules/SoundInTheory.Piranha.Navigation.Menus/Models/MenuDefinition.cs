using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public class MenuDefinition
    {
        /// <summary>
        /// A unique identifier for the menu, used to retrieve it in templates
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Title of the menu for reference purposes - it's only displayed in the manager
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The maximum depth of items in the menu
        /// </summary>
        public int MaxDepth { get; set; }
    }
}
