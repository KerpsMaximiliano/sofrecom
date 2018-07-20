using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/purchaseOrders")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService purchaseOrderService;
        private readonly IFileService fileService;
        private readonly FileConfig fileConfig;
        private readonly ISessionManager sessionManager;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IFileService fileService, IOptions<FileConfig> fileOptions, ISessionManager sessionManager)
        {
            this.purchaseOrderService = purchaseOrderService;
            this.fileService = fileService;
            this.sessionManager = sessionManager;
            fileConfig = fileOptions.Value;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = purchaseOrderService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new PurchaseOrderEditModel(response.Data));
        }

        [HttpPost]
        public IActionResult Post([FromBody] PurchaseOrderModel model)
        {
            var response = purchaseOrderService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] PurchaseOrderModel model)
        {
            var response = purchaseOrderService.Update(model);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = purchaseOrderService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}/file")]
        public IActionResult DeleteFile(int id)
        {
            var response = purchaseOrderService.DeleteFile(id);

            return this.CreateResponse(response);
        }

        [HttpPost("{purchaseOrderId}/file")]
        public async Task<IActionResult> File(int purchaseOrderId)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                await purchaseOrderService.AttachFile(purchaseOrderId, response, file, sessionManager.GetUserName());
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }

        [HttpGet("export/{id}")]
        public IActionResult ExportFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.PurchaseOrdersPath);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpGet("{id}/file")]
        public IActionResult GetFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.PurchaseOrdersPath);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(GetStatuses());
        }

        [HttpPost("{id}/solfac/{solfacId}")]
        public IActionResult UpdateSolfac(int id, int solfacId)
        {
            var response = purchaseOrderService.UpdateSolfac(id, solfacId);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/adjustment")]
        public IActionResult MakeAdjustment(int id, [FromBody] IList<PurchaseOrderAmmountDetailModel> details)
        {
            var response = purchaseOrderService.MakeAdjustment(id, details);

            return this.CreateResponse(response);
        }

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody]PurchaseOrderStatusParams model)
        {
            var response = purchaseOrderService.ChangeStatus(id, model);

            return this.CreateResponse(response);
        }

        [HttpPost]
        [Route("{id}/close")]
        public IActionResult Close(int id, [FromBody]PurchaseOrderStatusParams model)
        {
            var response = purchaseOrderService.Close(id, model);

            return this.CreateResponse(response);
        }

        [HttpGet]
        [Route("pendings")]
        public IActionResult Pendings()
        {
            var response = purchaseOrderService.GetPendings();

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var histories = purchaseOrderService.GetHistories(id);

            var list = histories.Select(x => new PurchaseOrderHistoryModel(x));

            return Ok(list);
        }

        private IEnumerable<Option> GetStatuses()
        {
            yield return new Option { Id = (int)PurchaseOrderStatus.Valid, Text = PurchaseOrderStatus.Valid.ToString() };
            yield return new Option { Id = (int)PurchaseOrderStatus.Consumed, Text = PurchaseOrderStatus.Consumed.ToString() };
            yield return new Option { Id = (int)PurchaseOrderStatus.Closed, Text = PurchaseOrderStatus.Closed.ToString() };
            yield return new Option { Id = (int)PurchaseOrderStatus.Draft, Text = PurchaseOrderStatus.Draft.ToString() };
            yield return new Option { Id = (int)PurchaseOrderStatus.ComercialPending, Text = PurchaseOrderStatus.ComercialPending.ToString() };
            yield return new Option { Id = (int)PurchaseOrderStatus.DafPending, Text = PurchaseOrderStatus.DafPending.ToString() };
            yield return new Option { Id = (int)PurchaseOrderStatus.OperativePending, Text = PurchaseOrderStatus.OperativePending.ToString() };
            yield return new Option { Id = (int)PurchaseOrderStatus.Reject, Text = PurchaseOrderStatus.Reject.ToString() };
            yield return new Option { Id = (int)PurchaseOrderStatus.CompliancePending, Text = PurchaseOrderStatus.CompliancePending.ToString() };
        }
    }
}