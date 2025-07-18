using Newtonsoft.Json;
using Piranha;
using Piranha.Models;
using SoundInTheory.Piranha.Navigation.Extensions;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Models.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IApi _api;

        public MenuRepository(IApi api)
        {
            _api = api;
        }

        public async Task<IEnumerable<Menu>> GetAll(Guid siteId)
        {
            var menus = await _api.Content.GetAllAsync<NavigationMenu>(NavigationMenu.ContentGroupId);
            return menus.Select(x => (Menu)x);
        }

        public async Task<IEnumerable<MenuInfo>> GetAllInfo(Guid siteId)
        {
            var menus = await _api.Content.GetAllAsync<NavigationMenu>(NavigationMenu.ContentGroupId);
            return menus.Select(x => (MenuInfo)x);
        }

        public async Task<Menu> GetById(Guid id)
        {
            var menu = await _api.Content.GetByIdAsync<NavigationMenu>(id);
            return menu;
        }

        public async Task<Menu> GetBySlug(Guid siteId, string slug)
        {
            var menus = await _api.Content.GetAllAsync<NavigationMenu>(NavigationMenu.ContentGroupId).ContinueWith(t => t.Result.ToList());
            return (Menu)menus.Find(x => x.Slug == slug);
        }

        public async Task<MenuInfo> GetInfoBySlug(Guid siteId, string slug)
        {
            var menus = await _api.Content.GetAllAsync<NavigationMenu>(NavigationMenu.ContentGroupId).ContinueWith(t => t.Result.ToList());
            return (MenuInfo)menus.Find(x => x.Slug == slug);
        }

        public async Task Save(MenuInfo model)
        {
            NavigationMenu menu = null;

            if (model.Id != Guid.Empty)
            {
                menu = await _api.Content.GetByIdAsync<NavigationMenu>(model.Id);
            }

            menu ??= await _api.Content.CreateAsync<NavigationMenu>(NavigationMenu.ContentTypeId);
            menu.Title = model.Title;
            menu.Slug = model.Slug;
            menu.IsSystemDefined = model.IsSystemDefined;
            menu.LastModified = DateTime.Now;
            menu.Settings = model.Settings;

            if (model is Menu menuModel)
            {
                menu.Items = menuModel.Items;
            }

            await _api.Content.SaveAsync(menu);
        }

        public async Task Delete(Guid menuId)
        {
            await _api.Content.DeleteAsync(menuId);
        }

        public async Task<MenuItem> GetItem(Guid menuId, Guid itemId)
        {
            var menu = await _api.Content.GetByIdAsync<NavigationMenu>(menuId);
            return menu.Items.FindRecursive(i => i.Id == itemId);
        }

        public async Task SaveItem(MenuItem model, Guid menuId, TreeNodePosition position)
        {
            var menu = await _api.Content.GetByIdAsync<NavigationMenu>(menuId);

            if (menu == null)
            {
                throw new InvalidOperationException($"Menu with id {menuId} could not be found");
            }

            var menuItems = menu.Items;
            var item = model.Id != Guid.Empty ? menuItems.FindRecursive(i => i.Id == model.Id) : null;

            // Create new if it doesn't exist
            if (item == null)
            {
                // Create the appropriate concrete type based on the model type
                item = CreateMenuItemInstance(model.GetType());
                item.Id = Guid.NewGuid();
                item.Created = DateTime.Now;

                menuItems.InsertNode(item, position);
            }
            else
            {
                // If the type has changed, we need to replace the item
                if (item.GetType() != model.GetType())
                {
                    var newItem = CreateMenuItemInstance(model.GetType());
                    newItem.Id = item.Id;
                    newItem.Created = item.Created;
                    newItem.Children = item.Children;
                    newItem.Hidden = item.Hidden;
                    
                    // Replace in the tree manually
                    ReplaceMenuItemInTree(menuItems, item, newItem);
                    item = newItem;
                }
            }

            // Copy properties from model to item
            CopyMenuItemProperties(model, item);
            
            item.LastModified = menu.LastModified = DateTime.Now;
            menu.Items = menuItems;

            // Update the model
            model.Id = item.Id;
            model.Created = item.Created;
            model.LastModified = item.LastModified;

            // Save all changes
            await _api.Content.SaveAsync(menu);
        }

        /// <summary>
        /// Creates a new instance of the specified menu item type.
        /// </summary>
        private MenuItem CreateMenuItemInstance(Type menuItemType)
        {
            if (menuItemType == typeof(Models.LinkMenuItem))
            {
                return new Models.LinkMenuItem();
            }
            else if (menuItemType == typeof(Models.StaticMenuItem))
            {
                return new Models.StaticMenuItem();
            }
            else
            {
                // Try to create using reflection for custom types
                return (MenuItem)Activator.CreateInstance(menuItemType);
            }
        }

        /// <summary>
        /// Replaces an item in the menu tree with a new item.
        /// </summary>
        private void ReplaceMenuItemInTree(IList<MenuItem> items, MenuItem oldItem, MenuItem newItem)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == oldItem.Id)
                {
                    items[i] = newItem;
                    return;
                }
                
                if (items[i].Children != null && items[i].Children.Count > 0)
                {
                    ReplaceMenuItemInTree(items[i].Children, oldItem, newItem);
                }
            }
        }

        /// <summary>
        /// Copies properties from source to target menu item.
        /// </summary>
        private void CopyMenuItemProperties(MenuItem source, MenuItem target)
        {
            target.Hidden = source.Hidden;
            
            // Copy type-specific properties using reflection
            var sourceType = source.GetType();
            var targetType = target.GetType();
            
            if (sourceType == targetType)
            {
                var properties = sourceType.GetProperties()
                    .Where(p => p.CanRead && p.CanWrite && 
                               p.DeclaringType != typeof(MenuItem) && 
                               !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any());

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(source);
                    prop.SetValue(target, value);
                }
            }
        }

        public async Task SaveItemStructure(List<MenuItem> model, Guid menuId)
        {
            var menu = await _api.Content.GetByIdAsync<NavigationMenu>(menuId);

            if (menu == null)
            {
                throw new InvalidOperationException($"Menu with id {menuId} could not be found");
            }

            menu.Items = menu.Items.MatchStructure(model, out _);

            // Save all changes
            await _api.Content.SaveAsync(menu);
        }

        public async Task DeleteItem(Guid menuId, Guid itemId)
        {
            var menu = await _api.Content.GetByIdAsync<NavigationMenu>(menuId);

            if (menu == null)
            {
                throw new InvalidOperationException($"Menu with id {menuId} could not be found");
            }

            var item = menu.Items.FindRecursive(x => x.Id == itemId);

            if (item == null)
            {
                throw new InvalidOperationException($"Item with id {itemId} could not be found");
            }

            menu.Items.RemoveRecursive(item);

            await _api.Content.SaveAsync(menu);
        }
    }
}
