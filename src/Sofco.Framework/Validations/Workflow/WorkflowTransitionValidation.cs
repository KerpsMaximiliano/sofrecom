using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Validations.Workflow
{
    public class WorkflowTransitionValidation : IWorkflowTransitionValidation
    {
        private readonly IUnitOfWork unitOfWork;

        public WorkflowTransitionValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void ValidateAdd(WorkflowTransitionAddModel model, Response response)
        {
            if (model == null)
            {
                response.AddError(Resources.Workflow.Workflow.EntityNull);
                return;
            }

            if (!model.ActualWorkflowStateId.HasValue || model.ActualWorkflowStateId.Value == 0)
            {
                response.AddError(Resources.Workflow.Workflow.ActualStateIsRequired);
            }
            else if (unitOfWork.WorkflowRepository.StateExist(model.ActualWorkflowStateId.Value))
            {
                response.AddError(Resources.Workflow.Workflow.ActualStateNotFound);
            }


            if (!model.NextWorkflowStateId.HasValue || model.NextWorkflowStateId.Value == 0)
            {
                response.AddError(Resources.Workflow.Workflow.NextStateIsRequired);
            }
            else if (unitOfWork.WorkflowRepository.StateExist(model.NextWorkflowStateId.Value))
            {
                response.AddError(Resources.Workflow.Workflow.NextStateNotFound);
            }
        }
    }
}
