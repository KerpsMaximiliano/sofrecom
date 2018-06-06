using Sofco.Model.Models.Billing;
using System.Linq;

namespace Sofco.Core.Models.Billing
{
    public class PurchaseOrderSearchResult
    {
        public PurchaseOrderSearchResult(PurchaseOrder purchaseOrder)
        {
            Id = purchaseOrder.Id;
            Number = purchaseOrder.Number;
            Client = purchaseOrder.ClientExternalName;
            Status = purchaseOrder.Status.ToString();
            Ammount = purchaseOrder.Ammount;
            Balance = purchaseOrder.Balance;

            if (purchaseOrder.Currency != null)
            {
                CurrencyDescription = purchaseOrder.Currency.Text;
            }

            if (purchaseOrder.File != null)
            {
                FileId = purchaseOrder.File.Id;
                FileName = purchaseOrder.File.FileName;
                CreationDate = purchaseOrder.File.CreationDate.ToString("d");
            }

            Details = purchaseOrder.AmmountDetails.Select(x => new AmmountDetailItem { Currency = x.Currency.Text, Balance = x.Balance }).ToList();
        }

        public int Id { get; set; }

        public string Number { get; set; }

        public string Client { get; set; }

        public string Status { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string CreationDate { get; set; }

        public IList<AmmountDetailItem> Details { get; set; }

        public decimal Balance { get; set; }
    }
}
