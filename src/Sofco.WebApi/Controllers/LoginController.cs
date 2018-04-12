using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services;
using Sofco.Model.Users;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly ILoginService service;

        public LoginController(ILoginService service)
        {
            this.service = service;
        }

        [HttpPost]
        public IActionResult Login([FromBody]UserLogin userLogin)
        {
            var response = service.Login(userLogin);

            return this.CreateResponse(response);
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult RefreshToken([FromBody]UserLoginRefresh userLoginRefresh)
        {
            var response = service.Refresh(userLoginRefresh);

            return this.CreateResponse(response);
        }
    }
}
