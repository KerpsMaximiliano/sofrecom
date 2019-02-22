using Sofco.Core.DAL;
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

        public void Process(WorkflowEntity entity)
        {
            var advancementAndRefunds = unitOfWork.AdvancementRepository.GetAdvancementAndRefundByRefund(entity.Id);

            foreach (var advancementRefund in advancementAndRefunds)
            {
                unitOfWork.AdvancementRepository.DeleteAdvancementRefund(advancementRefund);
            }
           
            unitOfWork.Save();
        }
    }
}
