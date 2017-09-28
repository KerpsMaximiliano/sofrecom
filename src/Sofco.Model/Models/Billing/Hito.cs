using System.ComponentModel.DataAnnotations.Schema;

namespace Sofco.Model.Models.Billing
{
    public class Hito
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public int SolfacId { get; set; }
        public Solfac Solfac { get; set; }
        public string Currency { get; set; }
        public short Month { get; set; }

        public string ExternalProjectId { get; set; }
        public string ExternalHitoId { get; set; }

        [NotMapped]
        public string DescriptionOld { get; set; }

        [NotMapped]
        public decimal UnitPriceOld { get; set; }
    }
}
