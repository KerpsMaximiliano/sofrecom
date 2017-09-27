using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models;

namespace Sofco.WebApi.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly AzureAdConfig _azureAdOptions;

        public LoginController(IOptions<AzureAdConfig> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            var client = new RestClient($"https://login.windows.net/{_azureAdOptions.Tenant}/oauth2/token?api-version=1.1");

            IRestRequest request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", 
                $"username={model.UserName}%40tebrasofre.onmicrosoft.com"
                +$"&password={model.Password}"
                +$"&grant_type={_azureAdOptions.GrantType}"
                +$"&client_id={_azureAdOptions.ClientId}"
                +$"&resource={_azureAdOptions.Audience}", ParameterType.RequestBody);

            var tcs = new TaskCompletionSource<IRestResponse>();

            client.ExecuteAsync(request, r =>
            {
                tcs.SetResult(r);
            });

            var response = (RestResponse) await tcs.Task;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(response.Content);
            }

            return BadRequest();
        }
    }
}
