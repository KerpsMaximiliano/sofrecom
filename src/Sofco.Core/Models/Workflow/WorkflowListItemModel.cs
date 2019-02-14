using System;

namespace Sofco.Core.Models.Workflow
{
    public class WorkflowListItemModel
    {
        public WorkflowListItemModel(Domain.Models.Workflow.Workflow domain)
        {
            Id = domain.Id;
            Description = domain.Description;
            ModifiedBy = domain.ModifiedBy?.Name;
            ModifiedAt = domain.ModifiedAt;
            Version = domain.Version;
            Active = domain.Active;
        }

        public int Id { get; set; }

        public string Description { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedAt { get; set; }

        public string Version { get; set; }

        public bool Active { get; set; }
    }
}
