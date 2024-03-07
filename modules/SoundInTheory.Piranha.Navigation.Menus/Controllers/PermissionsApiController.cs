using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager;
using Piranha.Manager.Localization;
using Piranha.Manager.Models;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Controllers
{
    /// <summary>
    /// Api controller for getting menu related permissions.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/navigation/permissions")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class PermissionApiController : Controller
    {
        private readonly IAuthorizationService _auth;

        public PermissionApiController(IAuthorizationService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public async Task<MenuPermissions> Get()
        {
            var model = new MenuPermissions();

            model.EditItems = (await _auth.AuthorizeAsync(User, Permissions.MenuItemsEdit)).Succeeded;
            model.DeleteItems = (await _auth.AuthorizeAsync(User, Permissions.MenusItemsDelete)).Succeeded;

            return model;
        }
    }

}
