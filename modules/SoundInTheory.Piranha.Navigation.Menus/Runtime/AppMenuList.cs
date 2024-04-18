using Piranha.Manager;
using SoundInTheory.Piranha.Navigation.Enums;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Runtime
{
    public class AppMenuList : IAppMenuList
    {
        private readonly ConcurrentDictionary<string, MenuDefinition> _menus = new();

        public MenuDefinition[] GetAll() => _menus.Values.ToArray();

        public bool Register(string slug, string title, int maxDepth = 0, List<EditorMenuOption> editorMenuOptions = null)
        {
            return Register(new MenuDefinition { Slug = slug, Title = title, MaxDepth = maxDepth, EnabledOptions = editorMenuOptions });
        }

        public bool Register(MenuDefinition menu)
        {
            return _menus.TryAdd(menu.Slug, menu);
        }

        public MenuDefinition this[string slug]
        {
            get
            {
                return _menus.TryGetValue(slug, out var menu) ? menu : null;
            }
            set
            {
                Register(value);
            }
        }
    }
}
