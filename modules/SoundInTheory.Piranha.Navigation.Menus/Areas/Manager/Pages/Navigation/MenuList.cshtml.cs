using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoundInTheory.Piranha.Navigation.Services;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Areas.Manager.Pages
{
    [Authorize(Policy = Permissions.Menus)]
    public class MenuListModel : PageModel
    {
    }
}
