using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Framework.Workflow
{
    public class WorkflowManager : IWorkflowManager
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;

        public WorkflowManager(IUnitOfWork unitOfWork, IOptions<AppSetting> appSetting)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSetting.Value;
        }

        public void CloseAdvancementsAndRefunds(int entityId)
        {
            var data = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(entityId);

            CloseAdvancementsAndRefunds(data, entityId);
        }

        public void CloseAdvancementsAndRefunds(Tuple<IList<Refund>, IList<Advancement>> data, int entityId)
        {
            foreach (var refund in data.Item1)
            {
                if (refund.Id != entityId)
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

        public void CloseEntity(WorkflowEntity entity)
        {
            entity.InWorkflowProcess = false;
            unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(entity);
            unitOfWork.Save();
        }
    }
}
