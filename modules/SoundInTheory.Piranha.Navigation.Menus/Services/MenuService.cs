using Piranha;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Repositories;
using SoundInTheory.Piranha.Navigation.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _repo;

        private readonly IApi _api;

        public MenuService(IMenuRepository repo, IApi api)
        {
            _repo = repo;
            _api = api;
        }

        public async Task<IEnumerable<Menu>> GetAll(Guid siteId)
        {
            return await _repo.GetAll(siteId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<MenuInfo>> GetAllInfo(Guid siteId)
        {
            return await _repo.GetAllInfo(siteId).ConfigureAwait(false);
        }

        public async Task<Menu> GetById(Guid id)
        {
            var menu = await _repo.GetById(id).ConfigureAwait(false);
            await OnLoad(menu).ConfigureAwait(false);

            return menu;
        }

        public async Task<Menu> GetBySlug(Guid siteId, string slug)
        {
            var menu = await _repo.GetBySlug(siteId, slug).ConfigureAwait(false);
            await OnLoad(menu).ConfigureAwait(false);

            return menu;
        }

        public Task SyncDefinitions(Guid siteId) 
            => SyncDefinitions(siteId, App.Modules.Navigation().Menus.GetAll());
        
        public async Task SyncDefinitions(Guid siteId, params MenuDefinition[] definitions)
        {
            var fromDb = (await _repo.GetAll(siteId).ConfigureAwait(false)).ToList();

            // Ensure system defined menus are up to date
            foreach (var definition in definitions)
            {
                var menu = await CreateOrUpdateFromDefinition(siteId, definition).ConfigureAwait(false);
                fromDb.RemoveAll(x => x.Id == menu.Id);
            }

            // Clear any legacy system menus left in the db. Don't delete menus that have items, just in case
            foreach (var menu in fromDb.Where(m => m.IsSystemDefined))
            {
                if (menu.IsEmpty)
                {
                    await Delete(menu.Id);
                }
                else
                {
                    menu.IsSystemDefined = false;
                    await SaveInfo(menu);
                }
            }
        }

        public async Task SaveInfo(MenuInfo model)
        {
            App.Hooks.OnBeforeSave(model);

            await _repo.Save(model).ConfigureAwait(false);

            App.Hooks.OnAfterSave(model);
        }

        public async Task Save(Menu model)
        {
            App.Hooks.OnBeforeSave(model);

            await _repo.Save(model).ConfigureAwait(false);

            App.Hooks.OnAfterSave(model);
        }

        public async Task Delete(Guid id)
        {
            var model = await _repo.GetById(id);

            if (model != null)
            {
                App.Hooks.OnBeforeDelete(model);

                await _repo.Delete(id);

                App.Hooks.OnAfterDelete(model);
            }
        }

        public async Task SaveItemStructure(Menu model)
        {
            App.Hooks.OnBeforeSave(model);

            await _repo.SaveItemStructure(model.Items, model.Id).ConfigureAwait(false);

            App.Hooks.OnAfterSave(model);
        }

        public async Task SaveItem(MenuItem model, Guid menuId, TreeNodePosition position)
        {
            App.Hooks.OnBeforeSave(model);

            await _repo.SaveItem(model, menuId, position).ConfigureAwait(false);

            App.Hooks.OnAfterSave(model);
        }

        public async Task DeleteItem(Guid menuId, Guid itemId)
        {
            var item = await _repo.GetItem(menuId, itemId);

            if (item != null)
            {
                App.Hooks.OnBeforeSave(item);

                await _repo.DeleteItem(menuId, itemId).ConfigureAwait(false);

                App.Hooks.OnAfterSave(item);
            }
        }

        private async Task<MenuInfo> CreateOrUpdateFromDefinition(Guid siteId, MenuDefinition definition)
        {
            var menu = await _repo.GetInfoBySlug(siteId, definition.Slug).ConfigureAwait(false)
                ?? new MenuInfo { Slug = definition.Slug };

            if (
                menu.Title != definition.Title || 
                menu.Settings.MaxDepth != definition.MaxDepth || 
                menu.Settings.EnabledOptions != definition.EnabledOptions ||
                !menu.IsSystemDefined
            )
            {
                menu.Title = definition.Title;
                menu.IsSystemDefined = true;
                menu.Settings.MaxDepth = definition.MaxDepth;
                menu.Settings.EnabledOptions = definition.EnabledOptions;
                await SaveInfo(menu);
            }

            return menu;
        }

        private async Task OnLoad(Menu menu)
        {
            if (menu == null) return;

            await menu.Init(_api).ConfigureAwait(false);

            App.Hooks.OnLoad(menu);
        }
    }
}
