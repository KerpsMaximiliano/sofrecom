using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Relationships
{
    public class PurchaseOrderAnalytic
    {
        public int PurchaseOrderId { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        public int AnalyticId { get; set; }

        public Analytic Analytic { get; set; }
    }
}
