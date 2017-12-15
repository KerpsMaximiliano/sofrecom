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
            var result = service.Login(userLogin);

            if (result.HasErrors())
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult RefreshToken([FromBody]UserLoginRefresh userLoginRefresh)
        {
            var result = service.Refresh(userLoginRefresh);

            return result.CreateResponse(this);
        }
    }
}
