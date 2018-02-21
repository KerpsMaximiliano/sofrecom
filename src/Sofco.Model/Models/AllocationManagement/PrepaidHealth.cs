using System;
using Newtonsoft.Json;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class PrepaidHealth : BaseEntity, IEntityDate
    {
        public int HealthInsuranceCode { get; set; }

        public int PrepaidHealthCode { get; set; }

        public string Name { get; set; }

        public int Amount { get; set; }

        [JsonIgnore]
        public DateTime? Created { get; set; }

        [JsonIgnore]
        public DateTime? Modified { get; set; }
    }
}
