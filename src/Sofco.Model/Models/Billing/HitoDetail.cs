using System;
using System.ComponentModel.DataAnnotations.Schema;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.Billing
{
    public class HitoDetail : BaseEntity, IEntityDate
    {
        public string Description { get; set; }
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public int HitoId { get; set; }
        public Hito Hito { get; set; }

        [NotMapped]
        public string ExternalHitoId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
