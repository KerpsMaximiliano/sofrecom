using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Relationships
{
    public class PurchaseOrderAnalytic
    {
        public int PurchaseOrderId { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        public int AnalyticId { get; set; }

        public Analytic Analytic { get; set; }
    }
}
