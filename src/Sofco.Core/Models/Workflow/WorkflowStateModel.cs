using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.Workflow
{
   public class WorkflowStateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public int IdType { get; set; }
    }
}
