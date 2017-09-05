using System.Linq;
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _invoiceService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new InvoiceViewModel(response.Data);

            return Ok(model);
        }

        [HttpPost]
        public IActionResult Post([FromBody] InvoiceViewModel model)
        {
            var domain = model.CreateDomain();

            var response = _invoiceService.Add(domain);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("project/{projectId}")]
        public IActionResult GetInvoices(string projectId)
        {
            var invoices = _invoiceService.GetByProject(projectId);

            var model = invoices.Select(x => new InvoiceRowDetailViewModel(x));

            return Ok(model);
        }
    }
}
