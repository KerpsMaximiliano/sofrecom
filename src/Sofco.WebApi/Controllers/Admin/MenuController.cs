using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models.Admin;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/menu")]
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IUserService _userService;

        public MenuController(IMenuService menuService, IUserService userService)
        {
            _menuService = menuService;
            _userService = userService;
        }

        [HttpGet("{userName}")]
        public IActionResult Get(string userName)
        {
            var roleFunctionalities = _menuService.GetFunctionalitiesByUserName(userName);

            var response = new MenuResponse();

            foreach (var item in roleFunctionalities)
            {
                if (item.Functionality.Active && item.Functionality.Module.Active)
                {
                    var menu = new MenuModel
                    {
                        Description = item.Functionality.Description,
                        Functionality = item.Functionality.Code,
                        Module = item.Functionality.Module.Code
                    };

                    response.Menus.Add(menu);
                }
            }

            response.IsDirector = _userService.HasDirectorGroup(this.GetUserMail());

            return Ok(response);
        }
    }
}
