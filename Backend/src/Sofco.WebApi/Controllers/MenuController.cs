using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services;
using Sofco.WebApi.Models;
using System.Collections.Generic;

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

        [HttpGet]
        public IActionResult Get()
        {
            var menus = _menuService.GetMenu(1);

            var response = new List<MenuModel>();

            foreach (var item in menus)
            {
                var menuModel = new MenuModel(item);

                foreach (var module in item.Modules)
                {
                    menuModel.Modules.Add(new Option(module.Id, module.Description));
                }

                response.Add(menuModel);
            }

            return Ok(response);
        }
    }
}
