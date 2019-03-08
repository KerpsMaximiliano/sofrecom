using Sofco.Core.Models.Workflow;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.Workflow
{
    public interface IWorkflowStateService
    {
        IList<WorkflowState> GetAll();
        IList<WorkflowStateTypeListItemModel> GetTypes();
        Response<WorkflowState> Active(int id, bool active);
        Response<WorkflowStateModel> GetById(int id);
        Response Update(WorkflowStateModel model);
        Response Add(WorkflowStateModel model);
    }
}
