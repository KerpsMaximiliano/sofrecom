using System;
using Newtonsoft.Json;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.Billing
{
    public class SolfacDelegate : BaseEntity, IEntityDate
    {
        public Guid ServiceId { get; set; }

        public int UserId { get; set; }

        public string CreatedUser { get; set; }

        [JsonIgnore]
        public DateTime? Created { get; set; }

        [JsonIgnore]
        public DateTime? Modified { get; set; }
    }
}
