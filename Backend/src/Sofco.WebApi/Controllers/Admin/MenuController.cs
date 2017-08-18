using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services;
using Sofco.WebApi.Models;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/menu")]
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
            var menus = _menuService.GetMenu(userName);

            var response = new List<MenuModel>();

            foreach (var item in menus)
            {
                if(item.Modules.Any(x => x.RoleModuleFunctionality.Any(y => y.Functionality.Active)))
                {
                    var menuModel = new MenuModel(item);

                    foreach (var module in item.Modules)
                    {
                        var moduleDetail = new ModuleModelDetail(module);

                        foreach (var roleModuleFunct in module.RoleModuleFunctionality)
                        {
                            if (roleModuleFunct.Functionality.Active)
                                moduleDetail.Functionalities.Add(new Option<string>(roleModuleFunct.Functionality.Code, roleModuleFunct.Functionality.Description));
                        }

                        menuModel.Modules.Add(moduleDetail);
                    }

                    response.Add(menuModel);
                }
            }

            return Ok(response);
        }
    }
}
