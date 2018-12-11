using System.Collections.Generic;

namespace Sofco.Core.Models.Workflow
{
    public class WorkflowChangeStatusParameters
    {
        public int WorkflowId { get; set; }

        public int NextStateId { get; set; }

        public int EntityId { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
