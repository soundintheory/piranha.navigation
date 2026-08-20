using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Repositories
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Menu>> GetAll(Guid siteId);

        Task<IEnumerable<MenuInfo>> GetAllInfo(Guid siteId);

        Task<Menu> GetById(Guid id);

        Task<MenuInfo> GetInfoById(Guid id);

        Task<Guid?> GetIdForSlug(Guid siteId, string slug);

        Task Save(MenuInfo model);

        Task Delete(Guid id);

        Task SaveItem(MenuItem model, Guid menuId, TreeNodePosition position);

        Task SaveItemStructure(List<MenuItem> model, Guid menuId);

        Task<MenuItem> GetItem(Guid menuId, Guid itemId);

        Task DeleteItem(Guid menuId, Guid itemId);
    }
}
