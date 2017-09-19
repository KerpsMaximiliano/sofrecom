using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.WebApi.Models.Billing;
using System.Collections.Generic;
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

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/solfac")]
    [Authorize]
    public class SolfacController : Controller
    {
        private readonly IUtilsService _utilsService;
        private readonly ISolfacService _solfacService;
        private readonly EmailConfig _emailConfig;

        public SolfacController(IUtilsService utilsService, ISolfacService solfacService, IOptions<EmailConfig> emailConfig)
        {
            _utilsService = utilsService;
            _solfacService = solfacService;
            _emailConfig = emailConfig.Value;
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
                Provinces = new List<SelectListItem> {new SelectListItem { Value = "0", Text = "Seleccione una opción" }}
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

            var handleStatus = _solfacService.ChangeStatus(response.Data, SolfacStatus.PendingByManagementControl, _emailConfig, response.Data.UserApplicantId, string.Empty);

            response.AddMessages(handleStatus.Messages);

            return Ok(response);
        }

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody] StatusChangeViewModel model)
        {
            var response = _solfacService.ChangeStatus(id, model.Status, _emailConfig, model.UserId, model.Comment);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
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
    }
}
