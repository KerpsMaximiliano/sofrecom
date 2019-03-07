using Sofco.Domain.Models.Workflow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.Workflow
{
    public interface IWorkflowStateService
    {
        IList<WorkflowState> GetAll();
    }
}
