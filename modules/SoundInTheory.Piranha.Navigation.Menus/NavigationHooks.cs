using Piranha.Models;
using Piranha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Piranha.Runtime;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Rendering;

namespace SoundInTheory.Piranha.Navigation
{
    public sealed class NavigationHooks
    {

        /// <summary>
        /// Gets the hooks available for aliases.
        /// </summary>
        public HookManager.ServiceHooks<Menu> Menus { get; } = new HookManager.ServiceHooks<Menu>();

        /// <summary>
        /// Gets the hooks available for media.
        /// </summary>
        public HookManager.ServiceHooks<MenuItem> MenuItems { get; } = new HookManager.ServiceHooks<MenuItem>();

        public NavigationMenuDelegate OnRenderMenu { get; set; }

        public NavigationMenuItemDelegate OnRenderMenuItem { get; set; }

        public delegate void NavigationMenuDelegate(MenuViewModel context);

        public delegate void NavigationMenuItemDelegate(MenuItemViewModel context);
    }
}
