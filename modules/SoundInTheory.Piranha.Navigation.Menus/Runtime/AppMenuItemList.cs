using Microsoft.CodeAnalysis;
using Piranha;
using Piranha.Extend;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Attributes;
using SoundInTheory.Piranha.Navigation.Extensions;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoundInTheory.Piranha.Navigation.Runtime
{
    /// <summary>
    /// Implementation for managing registered menu item types.
    /// </summary>
    public class AppMenuItemList : IAppMenuItemList
    {
        private readonly ConcurrentDictionary<string, MenuItemDefinition> _menuItems = new();

        /// <summary>
        /// Gets all registered menu item definitions.
        /// </summary>
        /// <returns>Array of menu item definitions</returns>
        public MenuItemDefinition[] GetAll() => _menuItems.Values.ToArray();

        /// <summary>
        /// Registers a new menu item type.
        /// </summary>
        /// <typeparam name="T">The menu item type</typeparam>
        /// <param name="name">The display name for this menu item type</param>
        /// <param name="configure">Optional configuration action</param>
        /// <returns>True if registration was successful</returns>
        public bool Register<T>(Action<MenuItemDefinition> configure = null) where T : MenuItem
        {
            var definition = new MenuItemDefinition
            {
                Id = typeof(T).Name,
                Title = typeof(T).Name,
                Type = typeof(T),
                Fields = DiscoverFields(typeof(T)),
                CssClass = $"type-{typeof(T).Name}",
            };

            var attribute = typeof(T).GetTypeInfo().GetCustomAttribute<MenuItemTypeAttribute>();

            if (attribute != null)
            {
                if (!string.IsNullOrEmpty(attribute.Id))
                {
                    definition.Id = attribute.Id;
                }
                if (!string.IsNullOrEmpty(attribute.Title))
                {
                    definition.Title = attribute.Title;
                }
                if (!string.IsNullOrEmpty(attribute.Description))
                {
                    definition.Description = attribute.Description;
                }
                if (!string.IsNullOrEmpty(attribute.ListTitle))
                {
                    definition.ListTitle = attribute.ListTitle;
                }
                if (!string.IsNullOrEmpty(attribute.ListType))
                {
                    definition.ListType = attribute.ListType;
                }
                if (attribute.AllowedChildren != null)
                {
                    definition.AllowedChildren = attribute.AllowedChildren;
                }
                if (attribute.AllowedParents != null)
                {
                    definition.AllowedParents = attribute.AllowedParents;
                }
                if (attribute.MaxLevel > 0)
                {
                    definition.MaxLevel = attribute.MaxLevel;
                }
                if (attribute.MaxDepth >= 0)
                {
                    definition.MaxDepth = attribute.MaxDepth;
                }
                if (!string.IsNullOrEmpty(attribute.ViewPath))
                {
                    definition.ViewPath = attribute.ViewPath;
                }
                if (!string.IsNullOrEmpty(attribute.CssClass))
                {
                    definition.CssClass = attribute.CssClass;
                }
            }

            configure?.Invoke(definition);

            return Register(definition);
        }

        /// <summary>
        /// Registers a menu item definition.
        /// </summary>
        /// <param name="definition">The menu item definition</param>
        /// <returns>True if registration was successful</returns>
        public bool Register(MenuItemDefinition definition)
        {
            return _menuItems.TryAdd(definition.Id, definition);
        }

        /// <summary>
        /// Gets a menu item definition by id.
        /// </summary>
        /// <param name="typeId">The type name</param>
        /// <returns>The menu item definition or null if not found</returns>
        public MenuItemDefinition this[string typeId]
        {
            get => GetById(typeId);
        }

        /// <summary>
        /// Gets a menu item definition by id
        /// </summary>
        /// <returns>The menu item definition or null if not found</returns>
        public MenuItemDefinition GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return _menuItems.TryGetValue(id, out var definition) ? definition : null;
        }

        /// <summary>
        /// Gets a menu item definition by type
        /// </summary>
        /// <returns>The menu item definition or null if not found</returns>
        public MenuItemDefinition Get<T>() where T : MenuItem => GetByType(typeof(T));

        /// <summary>
        /// Gets a menu item definition by type
        /// </summary>
        /// <returns>The menu item definition or null if not found</returns>
        public MenuItemDefinition GetByType(Type type)
        {
            foreach (var item in _menuItems)
            {
                if (item.Value.Type == type)
                {
                    return item.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Discovers Piranha fields on the given type using reflection.
        /// </summary>
        /// <param name="type">The type to analyze</param>
        /// <returns>List of discovered fields</returns>
        private IList<ContentTypeField> DiscoverFields(Type type)
        {
            var fields = new List<ContentTypeField>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                // Skip inherited properties from MenuItem base class
                if (property.DeclaringType == typeof(MenuItem) || property.DeclaringType.IsAssignableFrom(typeof(MenuItem)))
                    continue;

                // Check if this is a Piranha field type
                if (typeof(IField).IsAssignableFrom(property.PropertyType))
                {
                    App.Fields.AutoRegister(property.PropertyType);

                    var appField = App.Fields.GetByType(property.PropertyType);

                    var field = new ContentTypeField
                    {
                        Id = property.Name,
                        Title = GetFieldTitle(property),
                        Type = appField.TypeName,
                        Settings = new Dictionary<string, object> {
                            { "Component", appField?.Component ?? "string-field" }
                        }
                    };

                    fields.Add(field);
                }
            }

            return fields;
        }

        /// <summary>
        /// Gets the display title for a field property.
        /// </summary>
        /// <param name="property">The property to get the title for</param>
        /// <returns>The field title</returns>
        private string GetFieldTitle(PropertyInfo property)
        {
            // Look for Field attribute first
            var fieldAttribute = property.GetCustomAttribute<FieldAttribute>();
            if (fieldAttribute != null && !string.IsNullOrEmpty(fieldAttribute.Title))
            {
                return fieldAttribute.Title;
            }

            // Fall back to property name with space separation
            return System.Text.RegularExpressions.Regex.Replace(property.Name, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
