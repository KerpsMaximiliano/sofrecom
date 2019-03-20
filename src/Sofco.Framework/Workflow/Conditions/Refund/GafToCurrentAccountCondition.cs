using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Conditions.Refund
{
    public class GafToCurrentAccountCondition : IWorkflowConditionState
    {
        private readonly IUnitOfWork unitOfWork;

        public GafToCurrentAccountCondition(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            var refund = unitOfWork.RefundRepository.GetById(entity.Id);

            return !refund.CashReturn && refund.AdvancementRefunds.Any();
        }
    }
}
