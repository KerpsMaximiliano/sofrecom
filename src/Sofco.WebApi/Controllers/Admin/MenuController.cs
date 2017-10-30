using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Admin;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/menu")]
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IUserService _userService;
        private readonly EmailConfig _emailConfig;

        public MenuController(IMenuService menuService, IUserService userService, IOptions<EmailConfig> emailConfig)
        {
            _menuService = menuService;
            _userService = userService;
            _emailConfig = emailConfig.Value;
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
            response.IsDaf = _userService.HasDafGroup(this.GetUserMail(), _emailConfig.DafCode);
            response.IsCdg = _userService.HasCdgGroup(this.GetUserMail(), _emailConfig.CdgCode);

            return Ok(response);
        }
    }
}
