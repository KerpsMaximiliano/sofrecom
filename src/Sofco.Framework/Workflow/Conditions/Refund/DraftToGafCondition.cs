using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Conditions.Refund
{
    public class DraftToGafCondition : IWorkflowConditionState
    {
        private readonly IUnitOfWork unitOfWork;

        public DraftToGafCondition(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            var refund = unitOfWork.RefundRepository.Get(entity.Id);

            if (refund == null) return false;

            return refund.CashReturn;
        }
    }
}
