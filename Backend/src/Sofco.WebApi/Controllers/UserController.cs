using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.WebApi.Config;

namespace Sofco.WebApi.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IOptions<ActiveDirectoryConfig> _config;

        public UserController(IOptions<ActiveDirectoryConfig> config)
        {
            _config = config;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login()
        {
            return Ok();
        }
    }
}
