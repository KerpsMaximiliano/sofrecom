using System;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class ReportPowerBi : BaseEntity
    {
        public string Resource { get; set; }

        public string Seniority { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }

        public string Manager { get; set; }

        public decimal Month1 { get; set; }

        public decimal Month2 { get; set; }

        public decimal Month3 { get; set; }

        public decimal Month4 { get; set; }

        public string FirstMonth { get; set; }

        public string Comment { get; set; }
    }
}
