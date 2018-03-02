using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models.Admin;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/menu")]
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService menuService;
        private readonly IUserService userService;
        private readonly EmailConfig emailConfig;

        public MenuController(IMenuService menuService, IUserService userService, IOptions<EmailConfig> emailConfig)
        {
            this.menuService = menuService;
            this.userService = userService;
            this.emailConfig = emailConfig.Value;
        }

        [HttpGet("{userName}")]
        public IActionResult Get(string userName)
        {
            var roleFunctionalities = menuService.GetFunctionalitiesByUserName(userName);

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

            response.IsDirector = userService.HasDirectorGroup();
            response.IsDaf = userService.HasDafGroup();
            response.IsCdg = userService.HasCdgGroup();
            response.DafMail = menuService.GetGroupMail(emailConfig.DafCode);
            response.CdgMail = menuService.GetGroupMail(emailConfig.CdgCode);
            response.PmoMail = menuService.GetGroupMail(emailConfig.PmoCode);
            response.RrhhMail = menuService.GetGroupMail(emailConfig.RrhhCode);
            response.SellerMail = menuService.GetGroupMail(emailConfig.SellerCode);

            return Ok(response);
        }
    }
}
