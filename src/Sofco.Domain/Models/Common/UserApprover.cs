using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sofco.Common.Domains;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Models.Common
{
    public class UserApprover : BaseEntity, IEntityDate
    {
        public Analytic Analytic { get; set; }

        public int AnalyticId { get; set; }

        public int ApproverUserId { get; set; }

        public User ApproverUser { get; set; }

        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserApproverType Type { get; set; }

        public DateTime? Created { get; set; }

        public string CreatedUser { get; set; }

        public DateTime? Modified { get; set; }

        public string ModifiedUser { get; set; }
    }
}
