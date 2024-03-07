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
            var menus = await _api.Content.GetAllAsync<NavigationMenu>();
            return menus.Select(x => (Menu)x);
        }

        public async Task<IEnumerable<MenuInfo>> GetAllInfo(Guid siteId)
        {
            var menus = await _api.Content.GetAllAsync<NavigationMenu>();
            return menus.Select(x => (MenuInfo)x);
        }

        public async Task<Menu> GetById(Guid id)
        {
            var menu = await _api.Content.GetByIdAsync<NavigationMenu>(id);
            return menu;
        }

        public async Task<Menu> GetBySlug(Guid siteId, string slug)
        {
            var menus = await _api.Content.GetAllAsync<NavigationMenu>().ContinueWith(t => t.Result.ToList());
            return (Menu)menus.Find(x => x.Slug == slug);
        }

        public async Task<MenuInfo> GetInfoBySlug(Guid siteId, string slug)
        {
            var menus = await _api.Content.GetAllAsync<NavigationMenu>().ContinueWith(t => t.Result.ToList());
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
                item = new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.Now
                };

                menuItems.InsertNode(item, position);
            }

            item.Link = new LinkTag(model.Link);
            item.LastModified = menu.LastModified = DateTime.Now;
            menu.Items = menuItems;

            // Update the model
            model.Id = item.Id;
            model.Created = item.Created;
            model.LastModified = item.LastModified;

            // Save all changes
            await _api.Content.SaveAsync(menu);
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
