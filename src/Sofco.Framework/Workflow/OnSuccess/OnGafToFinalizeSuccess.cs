using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
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

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            var advancementAndRefunds = unitOfWork.AdvancementRepository.GetAdvancementRefundByRefundId(entity.Id);

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
