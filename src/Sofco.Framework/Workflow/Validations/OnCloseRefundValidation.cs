using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class OnCloseRefundValidation : IWorkflowValidationState
    {
        private readonly IUnitOfWork unitOfWork;

        public OnCloseRefundValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            var data = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(entity.Id);
            var refund = data.Item1.SingleOrDefault(x => x.Id == entity.Id);

            if (refund != null && !refund.CashReturn)
            {
                var refunds = data.Item1.Where(x => x.Id != entity.Id);

                if (refunds.Any(x => x.InWorkflowProcess))
                {
                    response.AddError(Resources.Workflow.Workflow.AllRefundsMustHaveSameStatus);
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
