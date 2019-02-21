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
            var advancements = unitOfWork.AdvancementRepository.GetByRefund(entity.Id);

            foreach (var advancement in advancements)
            {
                advancement.RefundId = null;
                unitOfWork.AdvancementRepository.UpdateRefundId(advancement);
            }
           
            unitOfWork.Save();
        }
    }
}
