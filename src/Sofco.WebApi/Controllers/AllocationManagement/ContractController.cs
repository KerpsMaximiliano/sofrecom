using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/contract")]
    [Authorize]
    public class ContractController : Controller
    {
        private readonly IContractService contractService;

        public ContractController(IContractService contractService)
        {
            this.contractService = contractService;
        }

        [HttpGet("accounts/{year}/{month}")]
        public IActionResult GetAccountsInfo(int year, int month)
        {
            var response = contractService.GetAccountsInfo(year, month);

            return this.CreateResponse(response);
        }
    }
}
