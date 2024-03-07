using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SoundInTheory.Piranha.Navigation.Areas.Manager.Pages.Navigation
{
    [Authorize(Policy = Permissions.Menus)]
    public class MenuEditModel : PageModel
    {
    }
}
