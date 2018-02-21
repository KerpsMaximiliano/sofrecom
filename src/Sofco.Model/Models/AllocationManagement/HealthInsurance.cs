using System;
using Newtonsoft.Json;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class HealthInsurance : BaseEntity, IEntityDate
    {
        public int Code { get; set; }

        public string Name { get; set; }
        
        [JsonIgnore]
        public DateTime? Created { get; set; }

        [JsonIgnore]
        public DateTime? Modified { get; set; }
    }
}
