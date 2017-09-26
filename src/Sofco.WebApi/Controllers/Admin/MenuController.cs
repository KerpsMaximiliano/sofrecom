using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/menu")]
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("{userName}")]
        public IActionResult Get(string userName)
        {
            var roleFunctionalities = _menuService.GetFunctionalitiesByUserName(userName);

            var response = new List<MenuModel>();

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

                    response.Add(menu);
                }
            }

            return Ok(response);
        }
    }
}
