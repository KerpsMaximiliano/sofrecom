using System;

namespace Sofco.Core.Models.Workflow
{
    public class WorkflowAddModel
    {
        public string Description { get; set; }

        public string Version { get; set; }

        public int? Type { get; set; }

        public Domain.Models.Workflow.Workflow CreateDomain()
        {
            var domain = new Domain.Models.Workflow.Workflow();

            domain.Description = Description;
            domain.Version = Version;
            domain.WorkflowTypeId = Type.GetValueOrDefault();

            domain.Active = true;

            domain.ModifiedAt = DateTime.UtcNow;
            domain.CreatedAt = DateTime.UtcNow;

            return domain;
        }
    }
}
