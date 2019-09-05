using Sofco.Core.DAL;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;
 
namespace Sofco.Framework.Workflow.Conditions.Refund
{
    public class HasUserRefundCondition : IWorkflowConditionState
    {
        private readonly IRefundValidation validation;

        private readonly IUnitOfWork unitOfWork;

        public HasUserRefundCondition(IRefundValidation validation, IUnitOfWork unitOfWork)
        {
            this.validation = validation;
            this.unitOfWork = unitOfWork;
        }

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            var refund = unitOfWork.RefundRepository.GetFullById(entity.Id);

            if (refund.CreditCardId.GetValueOrDefault() > 0) return false;

            return validation.HasUserRefund(refund);
        }
    }
}
