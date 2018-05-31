using Sofco.Model.Utils;
using PurchaseOrder = Sofco.Model.Models.Billing.PurchaseOrder;

namespace Sofco.WebApi.Models.Billing
{
    public class PurchaseOrderListItem
    {
        public PurchaseOrderListItem(PurchaseOrder purchaseOrder)
        {
            Id = purchaseOrder.Id;
            Number = purchaseOrder.Number;
            Client = purchaseOrder.ClientExternalName;
            Status = purchaseOrder.Status.ToString();
            Ammount = purchaseOrder.Ammount;

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
        }

        public int Id { get; set; }

        public string Number { get; set; }

        public string Client { get; set; }

        public string Status { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string CreationDate { get; set; }

        public string CurrencyDescription { get; set; }

        public decimal Ammount { get; set; }
    }
}
