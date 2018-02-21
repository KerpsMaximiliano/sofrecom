using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Model.DTO;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using PurchaseOrder = Sofco.Model.Models.Billing.PurchaseOrder;

namespace Sofco.Core.Services.Billing
{
    public interface IPurchaseOrderService
    {
        PurchaseOrderOptions GetFormOptions();
        Response<PurchaseOrder> Add(PurchaseOrder domain);
        Task<Response<File>> AttachFile(int purchaseOrderId, Response<File> response, IFormFile file, string userName);
        Response<PurchaseOrder> GetById(int id);
        Response Update(PurchaseOrder domain);
        ICollection<PurchaseOrder> Search(SearchPurchaseOrderParams parameters);
        Response DeleteFile(int id);
        ICollection<PurchaseOrder> GetByService(string serviceId);
    }
}
