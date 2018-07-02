using System;

namespace Sofco.Core.Models.Billing
{
    public class PurchaseOrderDelegateModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string CreatedUser { get; set; }

        public DateTime? Created { get; set; }
    }
}
