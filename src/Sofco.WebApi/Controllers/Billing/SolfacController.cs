using System;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.WebApi.Models.Billing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.WebApi.Config;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/solfacs")]
    [Authorize]
    public class SolfacController : Controller
    {
        private readonly IUtilsService _utilsService;
        private readonly ISolfacService _solfacService;
        private readonly EmailConfig _emailConfig;
        private readonly CrmConfig _crmConfig;

        public SolfacController(IUtilsService utilsService, ISolfacService solfacService, IOptions<EmailConfig> emailConfig, IOptions<CrmConfig> crmOptions)
        {
            _utilsService = utilsService;
            _solfacService = solfacService;
            _emailConfig = emailConfig.Value;
            _crmConfig = crmOptions.Value;
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search([FromBody] SolfacParams parameters)
        {
            var solfacs = _solfacService.Search(parameters);

            if (!solfacs.Any())
            {
                var response = new Response();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.NotFounds, MessageType.Warning));
                return Ok(response);
            }

            var list = solfacs.Select(x => new SolfacSearchDetail(x));

            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _solfacService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new SolfacDetail(response.Data);

            var provinces = _utilsService.GetProvinces().Where(x => x.Id != 1 && x.Id != 2).ToList();

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
            var solfacs = _solfacService.GetByProject(projectId);

            var list = solfacs.Select(x => new SolfacSearchDetail(x));

            return Ok(list);
        }

        [HttpGet]
        [Route("options")]
        public IActionResult FormOptions()
        {
            var options = new SolfacOptions
            {
                Currencies = _utilsService.GetCurrencies().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList(),
                DocumentTypes = _utilsService.GetDocumentTypes().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList(),
                ImputationNumbers = _utilsService.GetImputationNumbers().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList(),
                Provinces = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Seleccione una opción" } }
            };

            var provinces = _utilsService.GetProvinces().Where(x => x.Id != 1 && x.Id != 2).ToList();

            foreach (var province in provinces)
                options.Provinces.Add(new SelectListItem { Value = province.Id.ToString(), Text = province.Text });

            return Ok(options);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SolfacViewModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = _solfacService.Add(domain);

            if (response.HasErrors()) return BadRequest(response);

            var solfacChangeStatusResponse = new SolfacChangeStatusResponse { HitoStatus = HitoStatus.Pending, Hitos = response.Data.Hitos.Select(x => x.ExternalHitoId).ToList() };
            ChangeHitoStatus(solfacChangeStatusResponse);

            return Ok(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] SolfacDetail model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = _solfacService.Update(domain, model.Comments);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("send")]
        public IActionResult Send([FromBody] SolfacViewModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = _solfacService.Add(domain);

            if (response.HasErrors()) return BadRequest(response);

            var solfacStatusParams = new SolfacStatusParams(response.Data.UserApplicantId, string.Empty, string.Empty, SolfacStatus.PendingByManagementControl);

            var handleStatus = _solfacService.ChangeStatus(response.Data, solfacStatusParams, _emailConfig);

            if (handleStatus.HasErrors())
            {
                response.AddMessages(handleStatus.Messages);
                return BadRequest(response);
            }

            ChangeHitoStatus(handleStatus.Data);

            return Ok(response);
        }

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody] SolfacStatusChangeViewModel model)
        {
            var solfacStatusParams = new SolfacStatusParams(model.UserId, model.Comment, model.InvoiceCode, model.Status, model.InvoiceDate);

            var response = _solfacService.ChangeStatus(id, solfacStatusParams, _emailConfig);

            if (response.HasErrors()) return BadRequest(response);

            ChangeHitoStatus(response.Data);

            return Ok(response);
        }

        private async void ChangeHitoStatus(SolfacChangeStatusResponse data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_crmConfig.Url);
                HttpResponseMessage response;

                foreach (var item in data.Hitos)
                {
                    try
                    {
                        var stringContent = new StringContent($"StatusCode={(int)data.HitoStatus}", Encoding.UTF8, "application/x-www-form-urlencoded");
                        response = await client.PutAsync($"/api/InvoiceMilestone/{item}", stringContent);

                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                    }
                }

            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _solfacService.Delete(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var histories = _solfacService.GetHistories(id);

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

                    var response = _solfacService.SaveFile(solfacId, fileAsArrayBytes, file.FileName);

                    if (response.HasErrors()) return BadRequest(response);

                    var responseFile = new Response<SolfacAttachmentViewModel>();
                    responseFile.Messages = response.Messages;
                    responseFile.Data = new SolfacAttachmentViewModel(response.Data);

                    return Ok(responseFile);

                }
                catch (Exception e)
                {
                    var error = new Response();
                    error.Messages.Add(new Message(Resources.es.Common.SaveFileError, MessageType.Error));
                    return BadRequest(error);
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("{solfacId}/files")]
        public IActionResult GetFiles(int solfacId)
        {
            var files = _solfacService.GetFiles(solfacId);

            return Ok(files.Select(x => new SolfacAttachmentViewModel(x)));
        }

        [HttpGet]
        [Route("file/{fileId}")]
        public IActionResult GetFile(int fileId)
        {
            try
            {
                var response = _solfacService.GetFileById(fileId);

                if (response.HasErrors()) return BadRequest(response);

                return File(response.Data.File, "application/octet-stream", response.Data.Name);
            }
            catch (Exception e)
            {
                var response = new Response();
                response.Messages.Add(new Message(Resources.es.Common.ExportFileError, MessageType.Error));
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
            var response = _solfacService.DeleteFile(id);

            if (response.HasErrors()) return BadRequest(response);

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
