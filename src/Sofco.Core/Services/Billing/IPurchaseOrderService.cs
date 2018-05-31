using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using PurchaseOrder = Sofco.Model.Models.Billing.PurchaseOrder;

namespace Sofco.Core.Services.Billing
{
    public interface IPurchaseOrderService
    {
        Response<PurchaseOrder> Add(PurchaseOrderModel model);
        Task<Response<File>> AttachFile(int purchaseOrderId, Response<File> response, IFormFile file, string userName);
        Response<PurchaseOrder> GetById(int id);
        Response Update(PurchaseOrderModel model);
        ICollection<PurchaseOrder> Search(SearchPurchaseOrderParams parameters);
        Response DeleteFile(int id);
        IList<PurchaseOrder> GetByService(string serviceId);
    }
}
