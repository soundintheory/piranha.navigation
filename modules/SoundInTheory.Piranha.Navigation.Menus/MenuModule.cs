using Piranha;
using Piranha.Extend;
using Piranha.Manager;
using Piranha.Security;
using SoundInTheory.Piranha.Navigation.Runtime;
using System.Collections.Generic;
using Piranha.Runtime;

namespace SoundInTheory.Piranha.Navigation
{
    public class MenuModule : IModule
    {
        private readonly List<PermissionItem> _permissions = new()
        {
            new() { Name = Permissions.Menus, Title = "List menus", Category = "Navigation", IsInternal = true },
            new() { Name = Permissions.MenuItemsEdit, Title = "Edit menu items", Category = "Navigation", IsInternal = true },
            new() { Name = Permissions.MenusItemsDelete, Title = "Delete menu item", Category = "Navigation", IsInternal = true }
        };

        private readonly NavigationHooks _hooks = new NavigationHooks();

        /// <summary>
        /// The singleton module instance.
        /// </summary>
        public static MenuModule Instance { get; private set; }

        /// <summary>
        /// Register system defined menus here so that they get automatically created and can be edited in the manager (but not deleted)
        /// </summary>
        public IAppMenuList Menus { get; private set; } = new AppMenuList();

        /// <summary>
        /// Gets the module author
        /// </summary>
        public string Author => "Sound in Theory";

        /// <summary>
        /// Gets the module name
        /// </summary>
        public string Name => "Navigation";

        /// <summary>
        /// Gets the module version
        /// </summary>
        public string Version => Utils.GetAssemblyVersion(GetType().Assembly);

        /// <summary>
        /// Gets the module description
        /// </summary>
        public string Description => "Manage navigation links and menus on your website";

        /// <summary>
        /// Gets the module package url
        /// </summary>
        public string PackageUrl => "";

        /// <summary>
        /// Gets the module icon url
        /// </summary>
        public string IconUrl => "/manager/Navigation/images/logo.svg";

        /// <summary>
        /// The hooks for the menu data model
        /// </summary>
        public static NavigationHooks Hooks => Instance._hooks;

        public void Init()
        {
            Instance = this;

            // Register permissions
            foreach (var permission in _permissions)
            {
                App.Permissions["Navigation"].Add(permission);
            }

            // Add manager menu items
            Menu.Items.Insert(1, new MenuItem
            {
                InternalId = "Navigation",
                Name = "Navigation",
                Css = "fas fa-sitemap",
                Policy = Permissions.Menus
            });

            Menu.Items["Navigation"].Items.Add(new MenuItem
            {
                InternalId = "NavigationMenus",
                Name = "Menus",
                Route = "~/manager/navigation/menus",
                Css = "fas fa-list",
                Policy = Permissions.Menus
            });
        }
    }
}
