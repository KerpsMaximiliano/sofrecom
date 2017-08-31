using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var menus = _menuService.GetMenu(userName);

            var response = new List<MenuModel>();

            foreach (var item in menus)
            {
                if(item.Modules.Any(x => x.RoleModule.Any(y => y.Module.Active)))
                {
                    var menuModel = new MenuModel(item);

                    foreach (var module in item.Modules)
                    {
                        var moduleDetail = new ModuleModelDetail(module);

                        foreach (var roleModuleFunct in module.ModuleFunctionality)
                        {
                            if (roleModuleFunct.Functionality.Active)
                                moduleDetail.Functionalities.Add(new SelectListItem { Value = roleModuleFunct.Functionality.Code, Text = roleModuleFunct.Functionality.Description });
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
