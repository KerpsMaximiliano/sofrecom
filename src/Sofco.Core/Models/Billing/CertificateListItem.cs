using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class CertificateListItem
    {
        public CertificateListItem(Certificate purchaseOrder)
        {
            Id = purchaseOrder.Id;
            Name = purchaseOrder.Name;
            Client = purchaseOrder.ClientExternalName;
            Year = purchaseOrder.Year;

            if (purchaseOrder.File != null)
            {
                FileId = purchaseOrder.File.Id;
                FileName = purchaseOrder.File.FileName;
                CreationDate = purchaseOrder.File.CreationDate.ToString("d");
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Client { get; set; }

        public int Year { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string CreationDate { get; set; }
    }
}
