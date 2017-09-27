using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.FileManager;
using Sofco.Core.Services.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/invoices")]
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceFileManager _invoiceFileManager;
        private readonly EmailConfig _emailConfig;

        public InvoiceController(IInvoiceService invoiceService, IInvoiceFileManager invoiceFileManager, IOptions<EmailConfig> emailConfig)
        {
            _invoiceService = invoiceService;
            _invoiceFileManager = invoiceFileManager;
            _emailConfig = emailConfig.Value;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _invoiceService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new InvoiceViewModel(response.Data);

            return Ok(model);
        }

        [HttpGet("{projectId}/options")]
        public IActionResult Get(string projectId)
        {
            var invoices = _invoiceService.GetOptions(projectId);

            var list = invoices.Select(x => new SelectListItem {Value = x.Id.ToString(), Text = x.InvoiceNumber});

            return Ok(list);
        }

        [HttpPost]
        public IActionResult Post([FromBody] InvoiceViewModel model)
        {
            var domain = model.CreateDomain();

            var response = _invoiceService.Add(domain, User.Identity.Name);

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

        [HttpPost]
        [Route("excel")]
        public IActionResult Excel([FromBody]InvoiceViewModel model)
        {
            try
            {
                var excel = _invoiceFileManager.CreateInvoiceExcel(model.CreateDomain());

                var fileName = string.Concat("remito_", DateTime.Now.ToString("d"));

                return File(excel.GetAsByteArray(), "application/octet-stream", fileName);
            }
            catch (Exception e)
            {
                var response = new Response();
                response.Messages.Add(new Message("Ocurrio un error al generar el excel", MessageType.Error));
                return BadRequest(response);
            }
        }

        [HttpGet("{invoiceId}/excel")]
        public IActionResult GetExcel(int invoiceId)
        {
            try
            {
                var response = _invoiceService.GetExcel(invoiceId);

                if (response.HasErrors()) return BadRequest(response);

                return File(response.Data.ExcelFile, "application/octet-stream", response.Data.ExcelFileName);
            }
            catch (Exception e)
            {
                var response = new Response();
                response.Messages.Add(new Message("Ocurrio un error al generar el excel", MessageType.Error));
                return BadRequest(response);
            }
        }

        [HttpGet("{invoiceId}/pdf")]
        public IActionResult GetPdf(int invoiceId)
        {
            try
            {
                var response = _invoiceService.GetPdf(invoiceId);

                if (response.HasErrors()) return BadRequest(response);

                return File(response.Data.PdfFile, "application/octet-stream", response.Data.PdfFileName);
            }
            catch (Exception e)
            {
                var response = new Response();
                response.Messages.Add(new Message("Ocurrio un error al generar el excel", MessageType.Error));
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("{invoiceId}/excel")]
        public IActionResult Excel(int invoiceId)
        {
            if (Request.Form.Files.Any())
            {
                try
                {
                    var response = _invoiceService.GetById(invoiceId);

                    if (response.HasErrors()) return BadRequest(response);

                    var file = Request.Form.Files.First();

                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        response.Data.ExcelFile = memoryStream.ToArray();
                    }

                    response = _invoiceService.SaveExcel(response.Data);

                    if (response.HasErrors()) return BadRequest(response);

                    return Ok(response);

                }
                catch (Exception e)
                {
                    var error = new Response();
                    error.Messages.Add(new Message("Ocurrio un error al generar el excel", MessageType.Error));
                    return BadRequest(error);
                }
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("{invoiceId}/Pdf")]
        public IActionResult Pdf(int invoiceId)
        {
            if (Request.Form.Files.Any())
            {
                try
                {
                    var response = _invoiceService.GetById(invoiceId);

                    if (response.HasErrors()) return BadRequest(response);

                    var file = Request.Form.Files.First();

                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        response.Data.PdfFile = memoryStream.ToArray();
                    }

                    response = _invoiceService.SavePdf(response.Data);

                    if (response.HasErrors()) return BadRequest(response);

                    return Ok(response);

                }
                catch (Exception e)
                {
                    var error = new Response();
                    error.Messages.Add(new Message("Ocurrio un error al generar el excel", MessageType.Error));
                    return BadRequest(error);
                }
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("{invoiceId}/annulment")]
        public IActionResult Annulment(int invoiceId)
        {
            var response = _invoiceService.Annulment(invoiceId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _invoiceService.Delete(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody] InvoiceStatusChangeViewModel model)
        {
            var response = _invoiceService.ChangeStatus(id, model.Status, _emailConfig, new InvoiceStatusParams { Comment = model.Comment, InvoiceNumber = model.InvoiceNumber });

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search([FromBody] InvoiceParams parameters)
        {
            var invoices = _invoiceService.Search(parameters);

            if (!invoices.Any())
            {
                var response = new Response();
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFounds, MessageType.Warning));
                return Ok(response);
            }

            var list = invoices.Select(x => new InvoiceSearchDetail(x));

            return Ok(list);
        }

        [HttpGet("status")]
        public IActionResult GetInvoiceStatuses()
        {
            return Ok(GetStatuses());
        }

        private IEnumerable<SelectListItem> GetStatuses()
        {
            yield return new SelectListItem { Value = ((int)InvoiceStatus.SendPending).ToString(), Text = InvoiceStatus.SendPending.ToString() };
            yield return new SelectListItem { Value = ((int)InvoiceStatus.Sent).ToString(), Text = InvoiceStatus.Sent.ToString() };
            yield return new SelectListItem { Value = ((int)InvoiceStatus.Rejected).ToString(), Text = InvoiceStatus.Rejected.ToString() };
            yield return new SelectListItem { Value = ((int)InvoiceStatus.Approved).ToString(), Text = InvoiceStatus.Approved.ToString() };
            yield return new SelectListItem { Value = ((int)InvoiceStatus.Related).ToString(), Text = InvoiceStatus.Related.ToString() };
            yield return new SelectListItem { Value = ((int)InvoiceStatus.Cancelled).ToString(), Text = InvoiceStatus.Cancelled.ToString() };
        }
    }
}
