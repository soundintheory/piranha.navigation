using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoundInTheory.Piranha.Navigation.Enums;
using SoundInTheory.Piranha.Navigation.Models;

namespace SoundInTheory.Piranha.Navigation.Runtime
{
    public interface IAppMenuList
    {
        /// <summary>
        /// Get all system defined menus that have been registered
        /// </summary>
        /// <returns></returns>
        MenuDefinition[] GetAll();

        /// <summary>
        /// Registers a system defined menu.<para />Shorthand for <see cref="Register(MenuDefinition)"/>
        /// </summary>
        /// <param name="slug">A unique identifier for the menu, used to retrieve it in templates</param>
        /// <param name="title">Title of the menu for reference purposes - it's only displayed in the manager</param>
        /// <param name="maxDepth">The maximum depth of items in the menu</param>
        /// <returns>Whether the menu was registered successfully</returns>
        bool Register(string slug, string title, int maxDepth = 0, List<EditorMenuOption> editorMenuOptions = null);

        /// <summary>
        /// Registers a system defined menu. The menu will automatically be added to the database and users won't be able to delete it 
        /// </summary>
        /// <returns>Whether the menu was registered successfully</returns>
        bool Register(MenuDefinition menu);

        /// <summary>
        /// Gets the menu with the given slug.
        /// </summary>
        MenuDefinition this[string slug] { get; set; }
    }
}
