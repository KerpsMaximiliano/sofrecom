using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/solfacs")]
    [Authorize]
    public class SolfacController : Controller
    {
        private readonly IUtilsService utilsService;
        private readonly ISolfacService solfacService;
        private readonly ICertificateService certificateService;
        private readonly EmailConfig emailConfig;
        private readonly IPurchaseOrderService purchaseOrderService;

        public SolfacController(IUtilsService utilsService, ISolfacService solfacService, IOptions<EmailConfig> emailConfig, ICertificateService certificateService, IPurchaseOrderService purchaseOrderService)
        {
            this.utilsService = utilsService;
            this.solfacService = solfacService;
            this.emailConfig = emailConfig.Value;
            this.certificateService = certificateService;
            this.purchaseOrderService = purchaseOrderService;
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search([FromBody] SolfacParams parameters)
        {
            var solfacs = solfacService.Search(parameters);

            if (!solfacs.Any())
            {
                var response = new Response();
                response.Messages.Add(new Message(Resources.Billing.Solfac.NotFounds, MessageType.Warning));
                return Ok(response);
            }

            var list = solfacs.Select(x => new SolfacSearchDetail(x));

            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = solfacService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            var model = new SolfacDetail(response.Data);

            var provinces = utilsService.GetProvinces().Where(x => x.Id != 1 && x.Id != 2).ToList();

            if (response.Data.Province1Id > 0)
                model.ProvinceName1 = provinces.FirstOrDefault(x => x.Id == response.Data.Province1Id)?.Text;

            if (response.Data.Province2Id > 0)
                model.ProvinceName2 = provinces.FirstOrDefault(x => x.Id == response.Data.Province2Id)?.Text;

            if (response.Data.Province3Id > 0)
                model.ProvinceName3 = provinces.FirstOrDefault(x => x.Id == response.Data.Province3Id)?.Text;

            return Ok(model);
        }

        [HttpGet("project/{projectId}")]
        public IActionResult Get(string projectId)
        {
            var solfacs = solfacService.GetByProject(projectId);

            var list = solfacs.Select(x => new SolfacSearchDetail(x));

            return Ok(list);
        }

        [HttpGet]
        [Route("options/{serviceId}")]
        public IActionResult FormOptions(string serviceId)
        {
            var options = new SolfacOptions
            {
                Currencies = utilsService.GetCurrencies().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList(),
                DocumentTypes = utilsService.GetDocumentTypes().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList(),
                ImputationNumbers = utilsService.GetImputationNumbers().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList(),
                Provinces = utilsService.GetProvinces().Where(x => x.Id != 1 && x.Id != 2).Select(x => new Option { Id = x.Id, Text = x.Text }).ToList(),
                PaymentTerms = utilsService.GetPaymentTerms().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList(),
                PurchaseOrders = purchaseOrderService.GetByServiceLite(serviceId).Select(x => new Option { Id = x.Id, Text = x.Number }).ToList()
            };

            return Ok(options);
        }

        [HttpPost]
        [Route("validate")]
        public IActionResult Validate([FromBody] SolfacDetail model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = solfacService.Validate(domain);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SolfacViewModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = solfacService.Post(domain, model.InvoicesId, model.CertificatesId);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] SolfacDetail model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = solfacService.Update(domain, model.Comments);

            return this.CreateResponse(response);
        }

        [HttpPut]
        [Route("send")]
        public IActionResult PutAndSend([FromBody] SolfacDetail model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = solfacService.Update(domain, model.Comments);

            if (response.HasErrors())
                return BadRequest(response);

            var solfacStatusParams = new SolfacStatusParams(model.UserApplicantId, SolfacStatus.PendingByManagementControl);

            var handleStatus = solfacService.ChangeStatus(domain, solfacStatusParams, emailConfig);

            if (handleStatus.HasErrors())
            {
                response.AddMessages(handleStatus.Messages);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("send")]
        public IActionResult Send([FromBody] SolfacViewModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = solfacService.CreateSolfac(domain, model.InvoicesId, model.CertificatesId);

            if (response.HasErrors())
                return BadRequest(response);

            var solfacStatusParams = new SolfacStatusParams(response.Data.UserApplicantId, SolfacStatus.PendingByManagementControl);

            var handleStatus = solfacService.ChangeStatus(response.Data, solfacStatusParams, emailConfig);

            if (handleStatus.HasErrors())
            {
                response.AddMessages(handleStatus.Messages);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody] SolfacStatusChangeModel model)
        {
            var solfacStatusParams = model.CreateStatusParams();

            var response = solfacService.ChangeStatus(id, solfacStatusParams, emailConfig);

            return this.CreateResponse(response);
        }

        [HttpPut]
        [Route("{id}/bill")]
        public IActionResult UpdateBill(int id, [FromBody] UpdateSolfacBill model)
        {
            var solfacStatusParams = model.CreateStatusParams();

            var response = solfacService.UpdateBill(id, solfacStatusParams);

            return this.CreateResponse(response);
        }

        [HttpPut]
        [Route("{id}/cash")]
        public IActionResult UpdateCash(int id, [FromBody] UpdateSolfacCash model)
        {
            var solfacStatusParams = model.CreateStatusParams();

            var response = solfacService.UpdateCashedDate(id, solfacStatusParams);

            return this.CreateResponse(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            var response = solfacService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpDelete]
        [Route("details/{id}")]
        public IActionResult DeleteDetail(int id)
        {
            var response = solfacService.DeleteDetail(id);

            return this.CreateResponse(response);
        }

        [HttpDelete]
        [Route("{id}/invoice/{invoiceId}")]
        public IActionResult DeleteInvoiceOfSolfac(int id, int invoiceId)
        {
            var response = solfacService.DeleteInvoice(id, invoiceId);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var histories = solfacService.GetHistories(id);

            var list = histories.Select(x => new SolfacHistoryModel(x));

            return Ok(list);
        }

        [HttpPost]
        [Route("{solfacId}/file")]
        public IActionResult Excel(int solfacId)
        {
            if (Request.Form.Files.Any())
            {
                try
                {
                    var file = Request.Form.Files.First();
                    byte[] fileAsArrayBytes;

                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        fileAsArrayBytes = memoryStream.ToArray();
                    }

                    var response = solfacService.SaveFile(solfacId, fileAsArrayBytes, file.FileName);

                    if (response.HasErrors())
                        return BadRequest(response);

                    var responseFile = new Response<SolfacAttachmentModel>
                    {
                        Messages = response.Messages,
                        Data = new SolfacAttachmentModel(response.Data)
                    };

                    return Ok(responseFile);
                }
                catch
                {
                    var error = new Response();
                    error.Messages.Add(new Message(Resources.Common.SaveFileError, MessageType.Error));
                    return BadRequest(error);
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("{solfacId}/files")]
        public IActionResult GetFiles(int solfacId)
        {
            var files = solfacService.GetFiles(solfacId);

            return Ok(files.Select(x => new SolfacAttachmentModel(x)));
        }

        [HttpGet]
        [Route("file/{fileId}/download")]
        public IActionResult DownloadFile(int fileId)
        {
            try
            {
                var response = solfacService.GetFileById(fileId);

                if (response.HasErrors())
                    return BadRequest(response);

                return File(response.Data.File, "application/octet-stream", response.Data.Name);
            }
            catch
            {
                var response = new Response();
                response.Messages.Add(new Message(Resources.Common.ExportFileError, MessageType.Error));
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("file/{fileId}")]
        public IActionResult GetFile(int fileId)
        {
            try
            {
                var response = solfacService.GetFileById(fileId);

                if (response.HasErrors())
                    return BadRequest(response);

                return Ok(response.Data.File);
            }
            catch
            {
                var response = new Response();
                response.Messages.Add(new Message(Resources.Common.ExportFileError, MessageType.Error));
                return BadRequest(response);
            }
        }

        [HttpGet("status")]
        public IActionResult GetSolfacStatuses()
        {
            return Ok(GetStatuses());
        }

        [HttpDelete]
        [Route("file/{id}")]
        public IActionResult DeleteFile(int id)
        {
            var response = solfacService.DeleteFile(id);

            return this.CreateResponse(response);
        }

        [HttpGet]
        [Route("{id}/invoices")]
        public IActionResult GetInvoices(int id)
        {
            var response = solfacService.GetInvoices(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response.Data);
        }

        [HttpPost]
        [Route("{id}/invoices")]
        public IActionResult AddInvoices(int id, [FromBody] IList<int> invoices)
        {
            var response = solfacService.AddInvoices(id, invoices);

            return this.CreateResponse(response);
        }

        [HttpGet]
        [Route("{id}/certificates")]
        public IActionResult GetCertificates(int id)
        {
            var certificates = certificateService.GetBySolfac(id);

            return Ok(certificates.Select(x => new CertificateFileModel(x)));
        }

        [HttpDelete]
        [Route("{id}/certificate/{certificateId}")]
        public IActionResult DeleteSolfacCertificate(int id, int certificateId)
        {
            var response = solfacService.DeleteSolfacCertificate(id, certificateId);

            return this.CreateResponse(response);
        }

        [HttpPost]
        [Route("{id}/certificates")]
        public IActionResult AddCertificates(int id, [FromBody] IList<int> certificates)
        {
            var response = solfacService.AddCertificates(id, certificates);

            if (response.HasErrors())
                return BadRequest(response);

            var responseModel = new Response<IList<CertificateFileModel>> { Messages = response.Messages, Data = new List<CertificateFileModel>() };

            foreach (var certificate in response.Data)
            {
                responseModel.Data.Add(new CertificateFileModel(certificate));
            }

            return Ok(responseModel);
        }

        private IEnumerable<Option> GetStatuses()
        {
            yield return new Option { Id = (int)SolfacStatus.SendPending, Text = SolfacStatus.SendPending.ToString() };
            yield return new Option { Id = (int)SolfacStatus.PendingByManagementControl, Text = SolfacStatus.PendingByManagementControl.ToString() };
            yield return new Option { Id = (int)SolfacStatus.ManagementControlRejected, Text = SolfacStatus.ManagementControlRejected.ToString() };
            yield return new Option { Id = (int)SolfacStatus.InvoicePending, Text = SolfacStatus.InvoicePending.ToString() };
            yield return new Option { Id = (int)SolfacStatus.Invoiced, Text = SolfacStatus.Invoiced.ToString() };
            yield return new Option { Id = (int)SolfacStatus.AmountCashed, Text = SolfacStatus.AmountCashed.ToString() };
            yield return new Option { Id = (int)SolfacStatus.RejectedByDaf, Text = SolfacStatus.RejectedByDaf.ToString() };
        }
    }
}
