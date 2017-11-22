using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/solfacs")]
    [Authorize]
    public class SolfacController : Controller
    {
        private readonly IUtilsService utilsService;
        private readonly ISolfacService solfacService;
        private readonly EmailConfig emailConfig;
        private readonly CrmConfig crmConfig;

        public SolfacController(IUtilsService utilsService, ISolfacService solfacService, IOptions<EmailConfig> emailConfig, IOptions<CrmConfig> crmOptions)
        {
            this.utilsService = utilsService;
            this.solfacService = solfacService;
            this.emailConfig = emailConfig.Value;
            crmConfig = crmOptions.Value;
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search([FromBody] SolfacParams parameters)
        {
            var solfacs = solfacService.Search(parameters, this.GetUserMail());

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
        [Route("options")]
        public IActionResult FormOptions()
        {
            var options = new SolfacOptions
            {
                Currencies = utilsService.GetCurrencies().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList(),
                DocumentTypes = utilsService.GetDocumentTypes().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList(),
                ImputationNumbers = utilsService.GetImputationNumbers().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList(),
                Provinces = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Seleccione una opción" } },
                PaymentTerms = utilsService.GetPaymentTerms().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList()
            };

            var provinces = utilsService.GetProvinces().Where(x => x.Id != 1 && x.Id != 2).ToList();

            foreach (var province in provinces)
                options.Provinces.Add(new SelectListItem { Value = province.Id.ToString(), Text = province.Text });

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

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SolfacViewModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var domain = model.CreateDomain();

            var result = solfacService.Post(domain, model.InvoicesId);

            if (result.HasErrors())
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut]
        public IActionResult Put([FromBody] SolfacDetail model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = solfacService.Update(domain, model.Comments);

            if (response.HasErrors())
                return BadRequest(response);

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

            var response = solfacService.Add(domain, model.InvoicesId);

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
        public IActionResult ChangeStatus(int id, [FromBody] SolfacStatusChangeViewModel model)
        {
            var solfacStatusParams = model.CreateStatusParams();

            var response = solfacService.ChangeStatus(id, solfacStatusParams, emailConfig);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}/bill")]
        public IActionResult UpdateBill(int id, [FromBody] UpdateSolfacBill model)
        {
            var solfacStatusParams = model.CreateStatusParams();

            var response = solfacService.UpdateBill(id, solfacStatusParams);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}/cash")]
        public IActionResult UpdateCash(int id, [FromBody] UpdateSolfacCash model)
        {
            var solfacStatusParams = model.CreateStatusParams();

            var response = solfacService.UpdateCashedDate(id, solfacStatusParams);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            var response = solfacService.Delete(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("details/{id}")]
        public IActionResult DeleteDetail(int id)
        {
            var response = solfacService.DeleteDetail(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{id}/invoice/{invoiceId}")]
        public IActionResult DeleteInvoiceOfSolfac(int id, int invoiceId)
        {
            var response = solfacService.DeleteInvoice(id, invoiceId);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var histories = solfacService.GetHistories(id);

            var list = histories.Select(x => new SolfacHistoryViewModel(x));

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

                    var responseFile = new Response<SolfacAttachmentViewModel>();
                    responseFile.Messages = response.Messages;
                    responseFile.Data = new SolfacAttachmentViewModel(response.Data);

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

            return Ok(files.Select(x => new SolfacAttachmentViewModel(x)));
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

                return File(response.Data.File, "application/octet-stream", response.Data.Name);
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

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
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

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        private IEnumerable<SelectListItem> GetStatuses()
        {
            yield return new SelectListItem { Value = ((int)SolfacStatus.SendPending).ToString(), Text = SolfacStatus.SendPending.ToString() };
            yield return new SelectListItem { Value = ((int)SolfacStatus.PendingByManagementControl).ToString(), Text = SolfacStatus.PendingByManagementControl.ToString() };
            yield return new SelectListItem { Value = ((int)SolfacStatus.ManagementControlRejected).ToString(), Text = SolfacStatus.ManagementControlRejected.ToString() };
            yield return new SelectListItem { Value = ((int)SolfacStatus.InvoicePending).ToString(), Text = SolfacStatus.InvoicePending.ToString() };
            yield return new SelectListItem { Value = ((int)SolfacStatus.Invoiced).ToString(), Text = SolfacStatus.Invoiced.ToString() };
            yield return new SelectListItem { Value = ((int)SolfacStatus.AmountCashed).ToString(), Text = SolfacStatus.AmountCashed.ToString() };
        }
    }
}
