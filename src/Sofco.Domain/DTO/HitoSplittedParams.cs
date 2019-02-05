using System;

namespace Sofco.Domain.DTO
{
    public class HitoParameters
    {
        public string ExternalHitoId { get; set; }
        public string Name { get; set; }
        public decimal? Ammount { get; set; }
        public decimal AmmountFirstHito { get; set; }
        public string StatusCode { get; set; }
        public DateTime? StartDate { get; set; }
        public int Month { get; set; }
        public string ProjectId { get; set; }
        public string OpportunityId { get; set; }
        public string ManagerId { get; set; }
        public string MoneyId { get; set; }
    }
}
