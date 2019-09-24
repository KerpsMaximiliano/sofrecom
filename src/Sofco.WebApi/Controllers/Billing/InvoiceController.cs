using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.FileManager;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/invoices")]
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService invoiceService;
        private readonly IInvoiceFileManager invoiceFileManager;
        private readonly EmailConfig emailConfig;
        private readonly ISessionManager sessionManager;
        private readonly IFileService fileService;
        private readonly FileConfig fileConfig;

        public InvoiceController(
            IInvoiceService invoiceService,
            IInvoiceFileManager invoiceFileManager,
            IOptions<EmailConfig> emailConfig,
            IOptions<FileConfig> fileOptions,
            IFileService fileService,
            ISessionManager sessionManager)
        {
            this.invoiceService = invoiceService;
            this.invoiceFileManager = invoiceFileManager;
            this.emailConfig = emailConfig.Value;
            this.fileService = fileService;
            this.sessionManager = sessionManager;
            fileConfig = fileOptions.Value;
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

            var list = invoices.Select(x => new Option { Id = x.Id, Text = x.PDfFileData?.FileName });

            return Ok(list);
        }

        [HttpPost]
        public IActionResult Post([FromBody] InvoiceViewModel model)
        {
            var domain = model.CreateDomain();

            var response = invoiceService.Add(domain, User.Identity.Name);

            return this.CreateResponse(response);
        }

        [HttpGet("project/{projectId}")]
        public IActionResult GetInvoices(string projectId)
        {
            var invoices = invoiceService.GetByProject(projectId);

            var model = invoices.Select(x => new InvoiceRowDetailModel(x));

            return Ok(model);
        }

        [HttpPost("{invoiceId}/file")]
        public async Task<IActionResult> File(int invoiceId)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                await invoiceService.AttachFile(invoiceId, response, file, sessionManager.GetUserName(), false);
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }

        [HttpPost("{invoiceId}/file/multiple")]
        public async Task<IActionResult> MultipleFile(int invoiceId)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                if (fileService.HasFile(invoiceId))
                {
                    var cloned = invoiceService.Clone(invoiceId);

                    await invoiceService.AttachFile(cloned.Data.Id, response, file, sessionManager.GetUserName(), true);
                }
                else
                {
                    await invoiceService.AttachFile(invoiceId, response, file, sessionManager.GetUserName(), true);
                }
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/excel/export")]
        public IActionResult ExportExcelFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.InvoicesExcelPath);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpGet("{id}/excel/new")]
        public IActionResult ExportNewExcelFile(int id)
        {
            var response = invoiceService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var excel = invoiceFileManager.CreateInvoiceExcel(response.Data);

            return File(excel.GetAsByteArray(), "application/octet-stream", string.Empty);
        }

        [HttpGet("{id}/pdf/export")]
        public IActionResult ExportPdfFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.InvoicesPdfPath);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpPost("export/all")]
        public IActionResult ExportAll([FromBody] IList<int> ids)
        {
            var response = invoiceService.GetZip(ids);

            if (response.HasErrors()) return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpGet("{id}/pdf")]
        public IActionResult GetPdfFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.InvoicesPdfPath);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = invoiceService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody] InvoiceStatusChangeModel model)
        {
            var response = invoiceService.ChangeStatus(id, model.Status, emailConfig, new InvoiceStatusParams { Comment = model.Comment, InvoiceNumber = model.InvoiceNumber, UserId = model.UserId });

            return this.CreateResponse(response);
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search([FromBody] InvoiceParams parameters)
        {
            var invoices = invoiceService.Search(parameters);

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

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var histories = invoiceService.GetHistories(id);

            var list = histories.Select(x => new InvoiceHistoryModel(x));

            return Ok(list);
        }

        [HttpPost("requestAnnulment")]
        public IActionResult RequestAnnulment([FromBody] InvoiceAnnulmentModel model)
        {
            var response = invoiceService.RequestAnnulment(model);

            return this.CreateResponse(response);
        }

        private IEnumerable<Option> GetStatuses()
        {
            yield return new Option { Id = (int)InvoiceStatus.SendPending, Text = InvoiceStatus.SendPending.ToString() };
            yield return new Option { Id = (int)InvoiceStatus.Sent, Text = InvoiceStatus.Sent.ToString() };
            yield return new Option { Id = (int)InvoiceStatus.Rejected, Text = InvoiceStatus.Rejected.ToString() };
            yield return new Option { Id = (int)InvoiceStatus.Approved, Text = InvoiceStatus.Approved.ToString() };
            yield return new Option { Id = (int)InvoiceStatus.Related, Text = InvoiceStatus.Related.ToString() };
            yield return new Option { Id = (int)InvoiceStatus.Cancelled, Text = InvoiceStatus.Cancelled.ToString() };
            yield return new Option { Id = (int)InvoiceStatus.RequestAnnulment, Text = InvoiceStatus.Cancelled.ToString() };
        }
    }
}
