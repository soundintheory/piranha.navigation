using SoundInTheory.Piranha.Navigation.Models;
using System;

namespace SoundInTheory.Piranha.Navigation.Runtime
{
    /// <summary>
    /// Interface for managing registered menu item types.
    /// </summary>
    public interface IAppMenuItemList
    {
        /// <summary>
        /// Gets all registered menu item definitions.
        /// </summary>
        /// <returns>Array of menu item definitions</returns>
        MenuItemDefinition[] GetAll();

        /// <summary>
        /// Registers a new menu item type.
        /// </summary>
        /// <typeparam name="T">The menu item type</typeparam>
        /// <param name="name">The display name for this menu item type</param>
        /// <param name="configure">Optional configuration action</param>
        /// <returns>True if registration was successful</returns>
        void Register<T>(Action<MenuItemDefinition> configure = null) where T : MenuItem;

        /// <summary>
        /// Registers a menu item definition.
        /// </summary>
        /// <param name="definition">The menu item definition</param>
        /// <returns>True if registration was successful</returns>
        void Register(MenuItemDefinition definition);

        /// <summary>
        /// Gets a menu item definition by type name.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <returns>The menu item definition or null if not found</returns>
        MenuItemDefinition this[string typeName] { get; }

        /// <summary>
        /// Gets a menu item definition by id
        /// </summary>
        /// <returns>The menu item definition or null if not found</returns>
        public MenuItemDefinition GetById(string id);

        /// <summary>
        /// Gets a menu item definition by type
        /// </summary>
        /// <returns>The menu item definition or null if not found</returns>
        public MenuItemDefinition Get<T>() where T : MenuItem;

        /// <summary>
        /// Gets a menu item definition by type
        /// </summary>
        /// <returns>The menu item definition or null if not found</returns>
        public MenuItemDefinition GetByType(Type type);

        /// <summary>
        /// Removes a menu item definition by id
        /// </summary>
        /// <returns>The removed menu item definition or null if not found</returns>
        MenuItemDefinition RemoveById(string id);

        /// <summary>
        /// Removes menu item definition by type
        /// </summary>
        /// <returns>The removed menu item definition or null if not found</returns>
        MenuItemDefinition Remove<T>() where T : MenuItem;

        /// <summary>
        /// Removes a menu item definition by type
        /// </summary>
        /// <returns>The removed menu item definition or null if not found</returns>
        MenuItemDefinition RemoveByType(Type type);

        /// <summary>
        /// If a menu item definition with the supplied type exists, the definition will be passed into the callback allowing easy configuration
        /// </summary>
        void Configure<T>(Action<MenuItemDefinition> configure = null) where T : MenuItem;

        /// <summary>
        /// If a menu item definition with the supplied type exists, the definition will be passed into the callback allowing easy configuration
        /// </summary>
        void ConfigureByType(Type type, Action<MenuItemDefinition> configure = null);

        /// <summary>
        /// If a menu item definition with the supplied id exists, the definition will be passed into the callback allowing easy configuration
        /// </summary>
        void ConfigureById(string id, Action<MenuItemDefinition> configure = null);
    }
}
