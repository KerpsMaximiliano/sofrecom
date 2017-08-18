using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services;
using Sofco.WebApi.Models;

namespace Sofco.WebApi.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var response = _userService.GetByMail(model.Mail);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
