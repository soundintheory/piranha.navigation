using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Piranha;
using Piranha.Manager;
using Piranha.Manager.Models;
using Piranha.Manager.Services;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Models.Manager;
using SoundInTheory.Piranha.Navigation.Services;
using SoundInTheory.Piranha.Navigation.Services.Manager;

namespace SoundInTheory.Piranha.Navigation.Controllers
{
    /// <summary>
    /// Api controller for menu management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/navigation/menus")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class MenuApiController : Controller
    {
        private readonly IMenuService _service;
        private readonly IApi _api;
        private readonly ManagerLocalizer _localizer;
        private readonly MenuContentService _menuContent;

        public MenuApiController(IMenuService service, IApi api, ManagerLocalizer localizer, MenuContentService menuContent)
        {
            _service = service;
            _api = api;
            _localizer = localizer;
            _menuContent = menuContent;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        [Route("list")]
        [HttpGet]
        [Authorize(Policy = Permissions.Menus)]
        public async Task<MenuListModel> List()
        {
            var siteId = (await _api.Sites.GetDefaultAsync()).Id;
            await _service.SyncDefinitions(siteId);

            // Only show system defined menus for now.
            // TODO: Show all menus and allow users to manage them
            var menus = (await _service.GetAllInfo(siteId)).Where(m => m.IsSystemDefined);

            return new MenuListModel
            {
                Menus = menus.ToArray()
            };
        }

        /// <summary>
        /// Gets the menu with the given id.
        /// </summary>
        [Route("{menuId:Guid}")]
        [HttpGet]
        [Authorize(Policy = Permissions.Menus)]
        public async Task<MenuEditModel> Get(Guid menuId)
        {
            var menu = await _service.GetById(menuId);

            return new MenuEditModel
            {
                Menu = menu,
                AvailableMenuItemTypes = await _menuContent.GetAllItemTypes()
            };
        }

        /// <summary>
        /// Creates or updates a menu item
        /// </summary>
        [Route("{menuId:Guid}/items")]
        [HttpPost]
        [Authorize(Policy = Permissions.MenuItemsEdit)]
        public async Task<MenuEditModel> SaveItem(Guid menuId, [FromBody] MenuItemEditModel model)
        {
            await _service.SaveItem(model.Item, menuId, model.Position ?? default);
            
            var menu = await _service.GetById(menuId);

            return new MenuEditModel
            {
                Menu = menu,
                Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = "The menu item was saved successfully"
                }
            };
        }

        /// <summary>
        /// When items are moved around, JS sends the whole tree structure so we can update ParentIDs and SortOrders on each item
        /// </summary>
        [Route("{menuId:Guid}/structure")]
        [HttpPost]
        [Authorize(Policy = Permissions.MenuItemsEdit)]
        public async Task<MenuEditModel> SaveStructure(Guid menuId, [FromBody] MenuEditModel model)
        {
            await _service.SaveItemStructure(model.Menu);
            var menu = await _service.GetById(menuId);

            return new MenuEditModel
            {
                Menu = menu,
                Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = "The menu structure was updated successfully"
                }
            };
        }

        /// <summary>
        /// Deletes the menu item with the given id.
        /// </summary>
        /// <param name="menuId">The menu id</param>
        /// <param name="itemId">The item id</param>
        /// <returns>The result of the operation</returns>
        [Route("{menuId:Guid}/items/{itemId:Guid}")]
        [HttpDelete]
        [Authorize(Policy = Permissions.MenusItemsDelete)]
        public async Task<StatusMessage> DeleteItem(Guid menuId, Guid itemId)
        {
            try
            {
                await _service.DeleteItem(menuId, itemId);
            }
            catch (ValidationException e)
            {
                // Validation did not succeed
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
            }
            catch
            {
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = _localizer.Page["An error occured while deleting the item"]
                };
            }

            return new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Page["The page was successfully deleted"]
            };
        }
    }
}
