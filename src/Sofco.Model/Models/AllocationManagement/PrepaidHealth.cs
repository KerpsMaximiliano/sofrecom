using System;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class PrepaidHealth : BaseEntity, IEntityDate
    {
        public int HealthInsuranceCode { get; set; }

        public int PrepaidHealthCode { get; set; }

        public string Name { get; set; }

        public int Amount { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
