using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;

namespace Sofco.Core.DAL.Billing
{
    public interface IPurchaseOrderRepository : IBaseRepository<PurchaseOrder>
    {
        bool Exist(int purchaseOrderId);
        PurchaseOrder GetById(int purchaseOrderId);
        void AddPurchaseOrderAnalytic(PurchaseOrderAnalytic purchaseOrderAnalytic);
        PurchaseOrder GetWithAnalyticsById(int purchaseOrderId);
        IList<PurchaseOrder> GetByService(string serviceId);
        void UpdateBalance(PurchaseOrderAmmountDetail detail);
        IList<PurchaseOrder> GetByServiceLite(string serviceId);
        bool HasAmmountDetails(int solfacCurrencyId, int solfacPurchaseOrderId);
        void UpdateInSolfac(int id, int solfacId);
        void UpdateStatus(PurchaseOrder purchaseOrder);
        void UpdateDetail(PurchaseOrderAmmountDetail detail);
        void UpdateAdjustment(PurchaseOrder purchaseOrder);
        void AddHistory(PurchaseOrderHistory history);
        IList<Analytic> GetByAnalyticsWithSectors(int purchaseOrderId);
        IList<Analytic> GetByAnalyticsWithManagers(int purchaseOrderId);
        ICollection<PurchaseOrderHistory> GetHistories(int id);
        IList<PurchaseOrder> GetPendings();
        bool ExistNumber(string number, int domainId);
        bool HasWorkflowStarted(int purchaseOrderId);
        IList<PurchaseOrder> GetForReport();
        decimal GetBalance(int purchaseOrderId, int solfacCurrencyId);
    }
}
