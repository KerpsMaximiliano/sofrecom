using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using PurchaseOrderDomain = Sofco.Domain.Models.Billing.PurchaseOrder;

namespace Sofco.Core.Services.Billing.PurchaseOrder
{
    public interface IPurchaseOrderService
    {
        Response<PurchaseOrderDomain> Add(PurchaseOrderModel model);

        Response Update(PurchaseOrderModel model);

        Response Delete(int id);

        Response<PurchaseOrderDomain> GetById(int id);

        IList<PurchaseOrderDomain> GetByService(string serviceId);

        IList<PurchaseOrderDomain> GetByServiceLite(string serviceId, string opportunityNumber);

        ICollection<PurchaseOrderHistory> GetHistories(int id);
        
        Response<IList<PurchaseOrderPendingModel>> GetPendings();
    }
}
