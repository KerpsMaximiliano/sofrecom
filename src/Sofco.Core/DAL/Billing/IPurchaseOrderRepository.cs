using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Billing
{
    public interface IPurchaseOrderRepository : IBaseRepository<PurchaseOrder>
    {
        bool Exist(int purchaseOrderId);
        PurchaseOrder GetById(int purchaseOrderId);
        ICollection<PurchaseOrder> Search(SearchPurchaseOrderParams parameters);
        void AddPurchaseOrderAnalytic(PurchaseOrderAnalytic purchaseOrderAnalytic);
        PurchaseOrder GetWithAnalyticsById(int purchaseOrderId);
        IList<PurchaseOrder> GetByService(string serviceId);
        void UpdateBalance(PurchaseOrderAmmountDetail detail);

        IList<PurchaseOrder> GetByServiceLite(string serviceId);
        bool HasAmmountDetails(int solfacCurrencyId, int solfacPurchaseOrderId);
    }
}
