using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class CloseCashReturnProcess : IOnTransitionSuccessState
    {
        private readonly IWorkflowManager workflowManager;
        private readonly IUnitOfWork unitOfWork;

        public CloseCashReturnProcess(IWorkflowManager workflowManager, IUnitOfWork unitOfWork)
        {
            this.workflowManager = workflowManager;
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            var refund = unitOfWork.RefundRepository.Get(entity.Id);

            if (refund.CashReturn)
            {
                this.workflowManager.CloseEntity(entity);
            }
            else
            {
                this.workflowManager.CloseAdvancementsAndRefunds(entity.Id);
            }
        }
    }
}
