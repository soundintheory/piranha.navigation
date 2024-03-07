using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public interface IMenuService
    {
        /// <summary>
        /// Get a list of all menus, including their items
        /// </summary>
        Task<IEnumerable<Menu>> GetAll(Guid siteId);

        /// <summary>
        /// Get a list of all menu info for the site - menu info does not contain the items
        /// </summary>
        Task<IEnumerable<MenuInfo>> GetAllInfo(Guid siteId);

        /// <summary>
        /// Get a full menu (including items) by id
        /// </summary>
        Task<Menu> GetById(Guid id);

        /// <summary>
        /// Get a full menu (including items) by slug
        /// </summary>
        Task<Menu> GetBySlug(Guid siteId, string slug);

        /// <summary>
        /// Sync the database with the list of menu definitions added to the navigation module
        /// </summary>
        Task SyncDefinitions(Guid siteId);

        /// <summary>
        /// Sync the database with a list of system defined menu definitions
        /// </summary>
        Task SyncDefinitions(Guid siteId, params MenuDefinition[] definitions);

        /// <summary>
        /// Saves basic menu info to the database
        /// </summary>
        Task SaveInfo(MenuInfo model);

        /// <summary>
        /// Saves a menu to the database
        /// </summary>
        Task Save(Menu model);

        /// <summary>
        /// Updates the structure of the items tree according to the ones provided
        /// </summary>
        Task SaveItemStructure(Menu model);

        /// <summary>
        /// Saves a single menu item
        /// </summary>
        /// <param name="model">The item data</param>
        /// <param name="after">The saved item can optionally be positioned after the item with this id</param>
        Task SaveItem(MenuItem model, Guid menuId, TreeNodePosition position);

        /// <summary>
        /// Deletes an item from a menu by id
        /// </summary>
        Task DeleteItem(Guid menuId, Guid itemId);
    }
}
