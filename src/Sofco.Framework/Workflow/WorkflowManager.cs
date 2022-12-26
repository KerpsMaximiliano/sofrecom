using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.RequestNote;

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

            if (data.Item2.Any()) CloseAdvancements(data.Item2);

            CloseRefunds(data.Item1);
        }

        public void CloseRefunds(IList<Refund> refunds)
        {
            foreach (var refund in refunds)
            {
                refund.StatusId = appSetting.WorkflowStatusFinalizedId;
                unitOfWork.WorkflowRepository.UpdateStatus(refund);

                refund.InWorkflowProcess = false;
                unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(refund);
            }
        }

        public void PayRefunds(IList<Refund> refunds)
        {
            foreach (var refund in refunds)
            {
                refund.StatusId = appSetting.WorkflowStatusApproveId;
                unitOfWork.WorkflowRepository.UpdateStatus(refund);

                refund.InWorkflowProcess = false;
                unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(refund);
            }
        }

        public void CloseAdvancements(IList<Advancement> advancements)
        {
            foreach (var advancement in advancements)
            {
                advancement.StatusId = appSetting.WorkflowStatusFinalizedId;
                advancement.InWorkflowProcess = false;

                unitOfWork.WorkflowRepository.UpdateStatus(advancement);
                unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(advancement);
            }

            unitOfWork.Save();
        }

        public void CloseRequestNote(RequestNote note)
        {
            if (note.StatusId != appSetting.WorkflowStatusNPCerrado)
            {
                note.StatusId = appSetting.WorkflowStatusNPCerrado;
                note.InWorkflowProcess = false;

                unitOfWork.WorkflowRepository.UpdateStatus(note);
                unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(note);


                unitOfWork.Save();
            }
        }
        public void PartialReceptionRequestNote(RequestNote note)
        {
            if (note.StatusId != appSetting.WorkflowStatusNPRecepcionParcial)
            {
                note.StatusId = appSetting.WorkflowStatusNPRecepcionParcial;

                unitOfWork.WorkflowRepository.UpdateStatus(note);

                unitOfWork.Save();
            }
        }

        public void CloseEntity(WorkflowEntity entity)
        {
            entity.InWorkflowProcess = false;
            unitOfWork.WorkflowRepository.UpdateInWorkflowProcess(entity);
            unitOfWork.Save();
        }
    }
}
