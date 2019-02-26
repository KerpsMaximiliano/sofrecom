using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnCloseRefundSuccess : IOnTransitionSuccessState
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;

        public OnCloseRefundSuccess(IUnitOfWork unitOfWork, AppSetting appSetting)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSetting;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            var data = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(entity.Id);

            foreach (var refund in data.Item1)
            {
                if (refund.Id != entity.Id)
                {
                    refund.StatusId = appSetting.WorkflowStatusFinalizedId;
                    unitOfWork.WorkflowRepository.UpdateStatus(refund);
                }
              
                refund.InWorkflowProcess = false;
                unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(refund);
            }

            foreach (var advancement in data.Item2)
            {
                advancement.StatusId = appSetting.WorkflowStatusFinalizedId;
                advancement.InWorkflowProcess = false;

                unitOfWork.WorkflowRepository.UpdateStatus(advancement);
                unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(advancement);
            }

            unitOfWork.Save();
        }
    }
}
