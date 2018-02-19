using System;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class HealthInsurance : BaseEntity, IEntityDate
    {
        public int Code { get; set; }

        public string Name { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
