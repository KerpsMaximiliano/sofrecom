using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/purchaseOrders")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService purchaseOrderService;
        private readonly IFileService fileService;
        private readonly FileConfig fileConfig;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IFileService fileService, IOptions<FileConfig> fileOptions)
        {
            this.purchaseOrderService = purchaseOrderService;
            this.fileService = fileService;
            this.fileConfig = fileOptions.Value;
        }

        [HttpGet("formOptions")]
        public IActionResult GetFormOptions()
        {
            return Ok(purchaseOrderService.GetFormOptions());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = purchaseOrderService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(new PurchaseOrderEditViewModel(response.Data));
        }


        [HttpPost]
        public IActionResult Post([FromBody] PurchaseOrderViewModel model)
        {
            var domain = model.CreateDomain(this.GetUserName());

            var response = purchaseOrderService.Add(domain);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] PurchaseOrderEditViewModel model)
        {
            var domain = model.CreateDomain(this.GetUserName());

            var response = purchaseOrderService.Update(domain);

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

                await purchaseOrderService.AttachFile(purchaseOrderId, response, file, this.GetUserName());
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

            if (response.HasErrors()) return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }
        
        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchPurchaseOrderParams parameters)
        {
            var purchaseOrders = purchaseOrderService.Search(parameters);

            var response = new Response<List<PurchaseOrderListItem>>();
            response.Data = purchaseOrders.Select(x => new PurchaseOrderListItem(x)).ToList();

            if (!purchaseOrders.Any())
                response.AddWarning(Resources.Billing.PurchaseOrder.SearchEmpty);

            return Ok(response);
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(GetStatuses());
        }

        private IEnumerable<SelectListItem> GetStatuses()
        {
            yield return new SelectListItem { Value = ((int)PurchaseOrderStatus.Valid).ToString(), Text = PurchaseOrderStatus.Valid.ToString() };
            yield return new SelectListItem { Value = ((int)PurchaseOrderStatus.Consumed).ToString(), Text = PurchaseOrderStatus.Consumed.ToString() };
        }
    }
}