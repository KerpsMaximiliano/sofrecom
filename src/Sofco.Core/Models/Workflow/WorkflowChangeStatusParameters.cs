using Sofco.Core.Models.RequestNote;
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

    public class WorkflowChangeStatusMasiveParameters : WorkflowChangeStatusParameters
    {
        public string Type { get; set; }

        public string UserApplicantName { get; set; }
    }

    public class WorkflowChangeRequestNoteParameters : WorkflowChangeStatusParameters
    {
        public RequestNoteModel RequestNote { get; set; }
    }

    public class WorkflowChangeBuyOrderParameters : WorkflowChangeStatusParameters
    {
        public BuyOrder.BuyOrderModel BuyOrder { get; set; }

        public int? NextStateIdRequestNote { get; set; }
    }
}
