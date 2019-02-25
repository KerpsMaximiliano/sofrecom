using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnGafToFinalizeSuccess : IOnTransitionSuccessState
    {
        private readonly IUnitOfWork unitOfWork;

        public OnGafToFinalizeSuccess(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity)
        {
            var advancementAndRefunds = unitOfWork.AdvancementRepository.GetAdvancementAndRefundByRefund(entity.Id);

            foreach (var advancementRefund in advancementAndRefunds)
            {
                advancementRefund.Advancement.StatusId = entity.StatusId;
                advancementRefund.Advancement.InWorkflowProcess = false;

                unitOfWork.AdvancementRepository.UpdateStatus(advancementRefund.Advancement);
            }

            unitOfWork.Save();
        }
    }
}
