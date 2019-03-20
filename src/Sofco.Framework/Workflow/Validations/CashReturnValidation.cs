using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class CashReturnValidation : IWorkflowValidationState
    {
        private readonly IUnitOfWork unitOfWork;

        public CashReturnValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            var refund = unitOfWork.RefundRepository.Get(entity.Id);
            var tuple = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(entity.Id);

            if (refund.CashReturn)
            {
                if (tuple.Item1.Sum(x => x.TotalAmmount) > tuple.Item2.Sum(x => x.Ammount))
                {
                    response.AddError(Resources.AdvancementAndRefund.Refund.AmmountCashReturnError);
                    return false;
                }
            }

            return true;
        }
    }
}
