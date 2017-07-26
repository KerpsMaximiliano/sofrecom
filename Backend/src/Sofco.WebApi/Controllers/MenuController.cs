using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services;
using Sofco.WebApi.Models;
using System.Linq;

namespace Sofco.WebApi.Controllers
{
    [Route("api/menu")]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpPost]
        public IActionResult Get([FromBody] RolesModel roles)
        {
            var menus = _menuService.GetMenuByRoleId(roles.roles.ToArray());

            var model = menus.Select(x => new MenuModel(x));

            return Ok(model);
        }
    }
}
