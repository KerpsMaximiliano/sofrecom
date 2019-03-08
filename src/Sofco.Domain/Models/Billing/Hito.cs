using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Sofco.Common.Domains;

namespace Sofco.Domain.Models.Billing
{
    public class Hito : BaseEntity, IEntityDate
    {
        public string Description { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public short Month { get; set; }

        public int SolfacId { get; set; }
        public Solfac Solfac { get; set; }

        public string ProjectId { get; set; }
        public string ExternalHitoId { get; set; }

        public List<HitoDetail> Details { get; set; }

        public string CurrencyId { get; set; }

        public string OpportunityId { get; set; }

        public string ManagerId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
