using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;

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
            if (entity is Refund || entity is Advancement)
            {
                var advancementAndRefunds = unitOfWork.AdvancementRepository.GetAdvancementRefundByRefundId(entity.Id);

                foreach (var advancementRefund in advancementAndRefunds)
                {
                    unitOfWork.AdvancementRepository.DeleteAdvancementRefund(advancementRefund);
                }

                unitOfWork.Save();
            }

            entity.UsersAlreadyApproved = string.Empty;
            unitOfWork.WorkflowRepository.UpdateUsersAlreadyApproved(entity);
            unitOfWork.Save();
        }
    }
}
