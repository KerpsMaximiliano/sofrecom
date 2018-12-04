using System;
using Sofco.Domain.Models.Workflow;

namespace Sofco.Domain.Interfaces
{
    public class WorkflowHistory : BaseEntity
    {
        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }

        public int StatusFromId { get; set; }
        public WorkflowState StatusFrom { get; set; }

        public int StatusToId { get; set; }
        public WorkflowState StatusTo { get; set; }

        public string Comment { get; set; }

        public virtual void SetEntityId(int entityId)
        {
        }
    }
}
