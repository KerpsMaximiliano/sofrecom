using System.ComponentModel.DataAnnotations.Schema;

namespace Sofco.Model.Models.Billing
{
    public class HitoDetail : BaseEntity
    {
        public string Description { get; set; }
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public int HitoId { get; set; }
        public Hito Hito { get; set; }

        [NotMapped]
        public string ExternalHitoId { get; set; }
    }
}
