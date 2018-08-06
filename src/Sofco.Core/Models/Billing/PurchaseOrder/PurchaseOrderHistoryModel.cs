using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderHistoryModel
    {
        public PurchaseOrderHistoryModel(PurchaseOrderHistory history)
        {
            CreatedDate = history.CreatedDate;
            Comment = history.Comment;
            StatusFrom = history.From;
            StatusTo = history.To;

            if (history.User != null)
                UserName = history.User.Name;
        }

        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }

        public string Comment { get; set; }

        public PurchaseOrderStatus StatusFrom { get; set; }

        public PurchaseOrderStatus StatusTo { get; set; }
    }
}
