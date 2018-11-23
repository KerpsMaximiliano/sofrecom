using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Workflow
{
    public class TransitionItemModel
    {
        public int WorkflowId { get; set; }

        public int NextStateId { get; set; }

        public string NextStateDescription { get; set; }

        public WorkflowStateType WorkFlowStateType { get; set; }
    }
}
