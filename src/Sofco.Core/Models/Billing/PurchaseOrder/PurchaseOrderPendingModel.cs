using System;
using Sofco.Model.Enums;

namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderPendingModel
    {
        public PurchaseOrderPendingModel()
        {
            
        }

        public PurchaseOrderPendingModel(Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
            Id = purchaseOrder.Id;
            Number = purchaseOrder.Number;
            Client = purchaseOrder.ClientExternalName;
            Status = purchaseOrder.Status;
            ReceptionDate = purchaseOrder.ReceptionDate;

            if (purchaseOrder.Area != null)
                Area = purchaseOrder.Area.Text;
        }

        public DateTime ReceptionDate { get; set; }

        public int Id { get; set; }

        public string Number { get; set; }

        public string Client { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public string Area { get; set; }
    }
}
