using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class PurchaseOrderListItem
    {
        public PurchaseOrderListItem(PurchaseOrder purchaseOrder)
        {
            Id = purchaseOrder.Id;
            Title = purchaseOrder.Title;
            Client = purchaseOrder.ClientExternalName;
            Year = purchaseOrder.Year;
            Status = purchaseOrder.Status.ToString();

            if (purchaseOrder.Analytic != null)
                Analytic = $"{purchaseOrder.Analytic.Title} - {purchaseOrder.Analytic.Name}";

            if (purchaseOrder.File != null)
            {
                FileId = purchaseOrder.File.Id;
                FileName = purchaseOrder.File.FileName;
                CreationDate = purchaseOrder.File.CreationDate.ToString("d");
            }
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Client { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public string Analytic { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string CreationDate { get; set; }
    }
}
