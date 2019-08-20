using System;

namespace Sofco.Domain.Crm
{
    public class CrmServiceUpdate
    {
        public Guid Id { get; set; }

        public Guid? ManagerId { get; set; }

        public string AnalyticTitle { get; set; }

        public int? ServiceTypeId { get; set; }

        public int? SoluctionTypeId { get; set; }

        public int? TechnologyTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }
    }
}
