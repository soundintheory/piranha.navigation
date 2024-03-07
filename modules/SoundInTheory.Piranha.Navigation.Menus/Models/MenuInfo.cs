using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public class MenuInfo
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
	    public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the slug
        /// </summary>
	    public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
	    public string Title { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
	    public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
	    public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the JSON serialized settings.
        /// </summary>
        public MenuSettings Settings { get; set; } = new MenuSettings();

        /// <summary>
        /// Gets/sets whether the menu is defined by the system
        /// </summary>
	    public bool IsSystemDefined { get; set; }
    }
}
