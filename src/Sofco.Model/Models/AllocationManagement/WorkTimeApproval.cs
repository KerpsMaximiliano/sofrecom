using System;
using Sofco.Common.Domains;
using Sofco.Model.Models.Admin;

namespace Sofco.Model.Models.AllocationManagement
{
    public class WorkTimeApproval : BaseEntity, IEntityDate
    {
        public Analytic Analytic { get; set; }
        public int AnalyticId { get; set; }

        public int ApprovalUserId { get; set; }

        public User ApprovalUser { get; set; }

        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime? Created { get; set; }

        public string CreatedUser { get; set; }

        public DateTime? Modified { get; set; }

        public string ModifiedUser { get; set; }
    }
}
