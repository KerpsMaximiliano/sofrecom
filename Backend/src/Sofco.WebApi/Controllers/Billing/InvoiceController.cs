using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/invoice")]
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] InvoiceViewModel model)
        {
            var domain = model.CreateDomain();

            var response = _invoiceService.Add(domain);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
