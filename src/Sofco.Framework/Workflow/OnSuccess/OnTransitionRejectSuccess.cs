using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnTransitionRejectSuccess : IOnTransitionSuccessState
    {
        private readonly IUnitOfWork unitOfWork;

        public OnTransitionRejectSuccess(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            var advancementAndRefunds = unitOfWork.AdvancementRepository.GetAdvancementRefundByRefundId(entity.Id);

            foreach (var advancementRefund in advancementAndRefunds)
            {
                unitOfWork.AdvancementRepository.DeleteAdvancementRefund(advancementRefund);
            }
           
            unitOfWork.Save();
        }
    }
}
