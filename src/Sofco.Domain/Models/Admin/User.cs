using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Admin
{
    public class User : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IList<UserGroup> UserGroups { get; set; }

        public IList<Solfac> Solfacs { get; set; }

        public IList<Invoice> Invoices { get; set; }

        public string ExternalManagerId { get; set; }

        public ICollection<Analytic> Analytics2 { get; set; }
        public ICollection<Analytic> Analytics3 { get; set; }

        public ICollection<License> Licenses { get; set; }

        public ICollection<WorkTime> WorkTimes1 { get; set; }

        public ICollection<Area> Areas { get; set; }

        public ICollection<Sector> Sectors { get; set; }

        public ICollection<Employee> Employees { get; set; }

        public IList<Advancement> Advancements { get; set; }
        public IList<Advancement> Advancements2 { get; set; }

        public IList<Workflow.Workflow> Workflows { get; set; }
        public IList<Workflow.Workflow> Workflows2 { get; set; }

        public IList<Workflow.WorkflowReadAccess> WorkflowReadAccesses { get; set; }
        public IList<Workflow.WorkflowReadAccess> WorkflowReadAccesses2 { get; set; }

        public IList<Workflow.WorkflowState> WorkflowStates { get; set; }
        public IList<Workflow.WorkflowState> WorkflowStates2 { get; set; }

        public IList<Workflow.WorkflowStateAccess> WorkflowStateAccesses { get; set; }
        public IList<Workflow.WorkflowStateAccess> WorkflowStateAccesses2 { get; set; }

        public IList<Workflow.WorkflowStateNotifier> WorkflowStateNotifiers { get; set; }
        public IList<Workflow.WorkflowStateNotifier> WorkflowStateNotifiers2 { get; set; }

        public IList<Workflow.WorkflowStateTransition> WorkflowStateTransitions { get; set; }
        public IList<Workflow.WorkflowStateTransition> WorkflowStateTransitions2 { get; set; }

        public IList<Workflow.WorkflowType> WorkflowTypes2 { get; set; }
        public IList<Workflow.WorkflowType> WorkflowTypes { get; set; }

        public IList<Refund> Refunds { get; set; }
        public IList<Refund> Refunds2 { get; set; }

        public ICollection<CostDetailResource> CostDetailResources { get; set; }

        public IList<Delegation> Delegations1 { get; set; }
        public IList<Delegation> Delegations2 { get; set; }

        public IList<JobSearch> JobSearchs { get; set; }
        public IList<JobSearch> JobSearchs2 { get; set; }
        public IList<Applicant> Applicants { get; set; }
        public IList<Applicant> Applicants2 { get; set; }

        public IList<JobSearchApplicant> JobSearchApplicants1 { get; set; }
    }
}
