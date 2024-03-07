using Piranha.Models;
using Piranha.Extend.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Piranha;
using Newtonsoft.Json;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using SoundInTheory.Piranha.Navigation.Extensions;

namespace SoundInTheory.Piranha.Navigation.Models.Content
{
    [ContentGroup(Id = ContentGroupId, Title = "Menus", IsHidden = true)]
    [ContentType(Id = ContentTypeId, Title = "Menu", UseExcerpt = false, UsePrimaryImage = false)]
    public class NavigationMenu : Content<NavigationMenu>
    {
        public const string ContentGroupId = "NavigationMenus";

        public const string ContentTypeId = "NavigationMenu";

        [Region]
        public MenuContent Content { get; set; }

        public string Slug
        {
            get => Content.Slug?.Value;
            set => Content.Slug = value;
        }

        public List<MenuItem> Items
        {
            get => Content.Items;
            set => Content.Items = value;
        }

        public MenuSettings Settings
        {
            get => Content.Settings;
            set => Content.Settings = value;
        }

        public bool IsSystemDefined
        {
            get => Content.IsSystemDefined?.Value == true;
            set => Content.IsSystemDefined = value;
        }

        public static implicit operator Menu(NavigationMenu menu)
        {
            return menu == null ? null : new Menu
            {
                Id = menu.Id,
                Slug = menu.Slug,
                Title = menu.Title,
                Settings = menu.Settings,
                Created = menu.Created,
                LastModified = menu.LastModified,
                IsSystemDefined = menu.IsSystemDefined,
                Items = menu.Items
            };
        }

        public static implicit operator MenuInfo(NavigationMenu menu)
        {
            return menu == null ? null : new MenuInfo
            {
                Id = menu.Id,
                Slug = menu.Slug,
                Title = menu.Title,
                Settings = menu.Settings,
                Created = menu.Created,
                LastModified = menu.LastModified,
                IsSystemDefined = menu.IsSystemDefined
            };
        }

        public class MenuContent
        {
            /// <summary>
            /// Gets/sets the slug
            /// </summary>
            [Field]
            public StringField Slug { get; set; }

            /// <summary>
            /// The menu items
            /// </summary>
            [Field]
            public MenuItemsField Items { get; set; }

            /// <summary>
            /// JSON serialized settings.
            /// </summary>
            [Field]
            public MenuSettingsField Settings { get; set; }

            /// <summary>
            /// Whether the menu is defined by the system
            /// </summary>
            [Field]
            public CheckBoxField IsSystemDefined { get; set; }
        }
    }
}
