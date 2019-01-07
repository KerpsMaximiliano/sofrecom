using System.Collections.Generic;

namespace Sofco.Domain.Models.Workflow
{
    public class UserSource : BaseEntity
    {
        public string Code { get; set; }

        public int SourceId { get; set; }

        public IList<WorkflowReadAccess> WorkflowReadAccesses { get; set; }

        public IList<WorkflowStateAccess> WorkflowStateAccesses { get; set; }

        public IList<WorkflowStateNotifier> WorkflowStateNotifiers { get; set; }
    }
}
