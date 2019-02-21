using System.Collections.Generic;
using System.Linq;

namespace Sofco.Core.Models.Workflow
{
    public class WorkflowDetailModel
    {
        public WorkflowDetailModel(Domain.Models.Workflow.Workflow workflow)
        {
            Id = workflow.Id;
            Description = workflow.Description;
            Version = workflow.Version;
            Active = workflow.Active;

            if (workflow.Transitions != null)
            {
                Transitions = workflow.Transitions.Select(x => new WorkflowTransitionModel(x)).ToList();
            }
        }

        public int Id { get; set; }

        public string Description { get; set; }

        public int Version { get; set; }

        public bool Active { get; set; }

        public IList<WorkflowTransitionModel> Transitions { get; set; }
    }
}
