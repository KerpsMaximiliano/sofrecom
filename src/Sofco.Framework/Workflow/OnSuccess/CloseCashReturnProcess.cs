using System.Linq;
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

            var data = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(entity.Id);

            if (refund.CashReturn)
            {
                if (data.Item1.Count == 1 && data.Item1.Sum(x => x.TotalAmmount) == data.Item2.Sum(x => x.Ammount))
                {
                    this.workflowManager.CloseAdvancementsAndRefunds(data, entity.Id);
                }
                else
                {
                    this.workflowManager.CloseEntity(entity);
                }
            }
            else
            {
                this.workflowManager.CloseAdvancementsAndRefunds(data, entity.Id);
            }
        }
    }
}
