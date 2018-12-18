using System;

namespace Sofco.Core.Models.AdvancementAndRefund.Advancement
{
    public class AdvancementSearchFinalizedModel
    {
        public int? ResourceId { get; set; }

        public int? TypeId { get; set; }

        public DateTime? DateSince  { get; set; }

        public DateTime? DateTo  { get; set; }
    }
}
