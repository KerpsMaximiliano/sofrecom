using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using PurchaseOrder = Sofco.Domain.Models.Billing.PurchaseOrder;

namespace Sofco.Core.Services.Billing
{
    public interface IPurchaseOrderService
    {
        Response<PurchaseOrder> Add(PurchaseOrderModel model);

        Task<Response<File>> AttachFile(int purchaseOrderId, Response<File> response, IFormFile file, string userName);

        Response<PurchaseOrder> GetById(int id);

        Response DeleteFile(int id);

        IList<PurchaseOrder> GetByService(string serviceId);

        IList<PurchaseOrder> GetByServiceLite(string serviceId, string opportunityNumber);

        Response Update(PurchaseOrderModel model);

        Response MakeAdjustment(int id, IList<PurchaseOrderAmmountDetailModel> details);

        Response ChangeStatus(int id, PurchaseOrderStatusParams model);

        ICollection<PurchaseOrderHistory> GetHistories(int id);

        Response Close(int id, PurchaseOrderStatusParams model);
        
        Response<IList<PurchaseOrderPendingModel>> GetPendings();
        Response Delete(int id);
    }
}
