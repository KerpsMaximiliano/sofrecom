using System.Collections.Generic;
using System.Linq;

namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderSearchResult
    {
        public PurchaseOrderSearchResult(Domain.Models.Billing.PurchaseOrder purchaseOrder)
        {
            Id = purchaseOrder.Id;
            Number = purchaseOrder.Number;
            Client = purchaseOrder.ClientExternalName;
            Status = purchaseOrder.Status.ToString();

            if (purchaseOrder.File != null)
            {
                FileId = purchaseOrder.File.Id;
                FileName = purchaseOrder.File.FileName;
                CreationDate = purchaseOrder.File.CreationDate.ToString("d");
            }

            Details = purchaseOrder.AmmountDetails.Select(x => new AmmountDetailModel { Currency = x.Currency.Text, Balance = x.Balance }).ToList();
        }

        public int Id { get; set; }

        public string Number { get; set; }

        public string Client { get; set; }

        public string Status { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string CreationDate { get; set; }

        public IList<AmmountDetailModel> Details { get; set; }
    }
}
