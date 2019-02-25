using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnPaymentPendingToFinalized : IOnTransitionSuccessState
    {
        private readonly IUnitOfWork unitOfWork;

        public OnPaymentPendingToFinalized(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity)
        {
            
        }
    }
}
