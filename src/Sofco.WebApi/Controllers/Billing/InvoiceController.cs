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
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/invoices")]
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService invoiceService;
        private readonly IInvoiceFileManager invoiceFileManager;
        private readonly EmailConfig emailConfig;

        public InvoiceController(IInvoiceService invoiceService, IInvoiceFileManager invoiceFileManager, IOptions<EmailConfig> emailConfig)
        {
            this.invoiceService = invoiceService;
            this.invoiceFileManager = invoiceFileManager;
            this.emailConfig = emailConfig.Value;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = invoiceService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            var model = new InvoiceViewModel(response.Data);

            return Ok(model);
        }

        [HttpGet("{projectId}/options")]
        public IActionResult GetOptions(string projectId)
        {
            var invoices = invoiceService.GetOptions(projectId);

            var list = invoices.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.PdfFileName });

            return Ok(list);
        }

        [HttpPost]
        public IActionResult Post([FromBody] InvoiceViewModel model)
        {
            var domain = model.CreateDomain();

            var response = invoiceService.Add(domain, User.Identity.Name);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("project/{projectId}")]
        public IActionResult GetInvoices(string projectId)
        {
            var invoices = invoiceService.GetByProject(projectId);

            var model = invoices.Select(x => new InvoiceRowDetailViewModel(x));

            return Ok(model);
        }

        [HttpPost]
        [Route("excel")]
        public IActionResult Excel([FromBody]InvoiceViewModel model)
        {
            try
            {
                var excel = invoiceFileManager.CreateInvoiceExcel(model.CreateDomain());

                var fileName = string.Concat("remito_", DateTime.Now.ToString("d"));

                return File(excel.GetAsByteArray(), "application/octet-stream", fileName);
            }
            catch
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
                var response = invoiceService.GetExcel(invoiceId);

                if (response.HasErrors())
                    return BadRequest(response);

                return File(response.Data.ExcelFile, "application/octet-stream", response.Data.ExcelFileName);
            }
            catch
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
                var response = invoiceService.GetPdf(invoiceId);

                if (response.HasErrors())
                    return BadRequest(response);

                return Ok(response.Data.PdfFile);
            }
            catch
            {
                var response = new Response();
                response.Messages.Add(new Message("Ocurrio un error al generar el excel", MessageType.Error));
                return BadRequest(response);
            }
        }

        [HttpGet("{invoiceId}/pdf/download")]
        public IActionResult DownloadPdf(int invoiceId)
        {
            try
            {
                var response = invoiceService.GetPdf(invoiceId);

                if (response.HasErrors())
                    return BadRequest(response);

                return File(response.Data.PdfFile, "application/octet-stream", response.Data.PdfFileName);
            }
            catch
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
                    var response = invoiceService.GetById(invoiceId);

                    if (response.HasErrors())
                        return BadRequest(response);

                    var file = Request.Form.Files.First();

                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        response.Data.ExcelFile = memoryStream.ToArray();
                    }

                    response = invoiceService.SaveExcel(response.Data, file.FileName);

                    if (response.HasErrors())
                        return BadRequest(response);

                    return Ok(response);
                }
                catch
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
                    var response = invoiceService.GetById(invoiceId);

                    if (response.HasErrors())
                        return BadRequest(response);

                    var file = Request.Form.Files.First();

                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        response.Data.PdfFile = memoryStream.ToArray();
                    }

                    response = invoiceService.SavePdf(response.Data, file.FileName);

                    if (response.HasErrors())
                        return BadRequest(response);

                    return Ok(response);
                }
                catch
                {
                    var error = new Response();
                    error.Messages.Add(new Message("Ocurrio un error al generar el excel", MessageType.Error));
                    return BadRequest(error);
                }
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = invoiceService.Delete(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody] InvoiceStatusChangeViewModel model)
        {
            var response = invoiceService.ChangeStatus(id, model.Status, emailConfig, new InvoiceStatusParams { Comment = model.Comment, InvoiceNumber = model.InvoiceNumber, UserId = model.UserId });

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search([FromBody] InvoiceParams parameters)
        {
            var invoices = invoiceService.Search(parameters, this.GetUserMail(), emailConfig);

            if (!invoices.Any())
            {
                var response = new Response();
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFounds, MessageType.Warning));
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

        [HttpPost]
        [Route("{id}/clone")]
        public IActionResult Clone(int id)
        {
            var response = invoiceService.Clone(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var histories = invoiceService.GetHistories(id);

            var list = histories.Select(x => new InvoiceHistoryViewModel(x));

            return Ok(list);
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
