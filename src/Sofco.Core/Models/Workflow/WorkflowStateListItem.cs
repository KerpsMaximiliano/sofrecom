using Sofco.Domain.Models.Workflow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.Workflow
{
    public class WorkflowStateListItem
    {
        
        public WorkflowStateListItem(WorkflowState domain)
        {
            this.Id = domain.Id;
            this.Name = domain.Name;
            this.ActionName = domain.ActionName;
            this.Type = domain.Type.ToString();
            this.IdType = (int)domain.Type;
            this.Active = domain.Active;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public string Type { get; set; }
        public int IdType { get; set; }
        public bool Active { get; set; }
    }
}
