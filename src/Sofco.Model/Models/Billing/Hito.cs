using System.Collections.Generic;

namespace Sofco.Model.Models.Billing
{
    public class Hito : BaseEntity
    {
        public string Description { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public short Month { get; set; }

        public int SolfacId { get; set; }
        public Solfac Solfac { get; set; }

        public string ExternalProjectId { get; set; }
        public string ExternalHitoId { get; set; }

        public List<HitoDetail> Details { get; set; }
    }
}
