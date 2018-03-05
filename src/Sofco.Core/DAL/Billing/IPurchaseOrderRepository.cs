using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface IPurchaseOrderRepository : IBaseRepository<PurchaseOrder>
    {
        bool Exist(int purchaseOrderId);
        PurchaseOrder GetById(int purchaseOrderId);
        ICollection<PurchaseOrder> Search(SearchPurchaseOrderParams parameters);
        ICollection<PurchaseOrder> GetByService(string serviceId);
    }
}
