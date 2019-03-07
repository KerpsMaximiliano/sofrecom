using System.Collections.Generic;

namespace Sofco.Core.Models.Workflow
{
    public class WorkflowTransitionAddModel
    {
        public int? Id { get; set; }

        public int? ActualWorkflowStateId { get; set; }

        public int? NextWorkflowStateId { get; set; }

        public int? WorkflowId { get; set; }

        public string NotificationCode { get; set; }

        public string ValidationCode { get; set; }

        public string ConditionCode { get; set; }

        public string ParameterCode { get; set; }

        public string OnSuccessCode { get; set; }

        public bool NotifyToUserApplicant { get; set; }

        public bool NotifyToManager { get; set; }

        public IList<int> NotifyToUsers { get; set; }

        public IList<int> NotifyToGroups { get; set; }

        public bool NotifyToSector { get; set; }

        public bool UserApplicantHasAccess { get; set; }

        public bool ManagerHasAccess { get; set; }

        public IList<int> UsersHasAccess { get; set; }

        public IList<int> GroupsHasAccess { get; set; }

        public bool SectorHasAccess { get; set; }
    }
}
