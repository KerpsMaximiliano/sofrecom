using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class FinalizeWorkflowProcess : IOnTransitionSuccessState
    {
        private readonly IUnitOfWork unitOfWork;

        public FinalizeWorkflowProcess(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            entity.InWorkflowProcess = false;
            unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(entity);
            unitOfWork.Save();
        }
    }
}
