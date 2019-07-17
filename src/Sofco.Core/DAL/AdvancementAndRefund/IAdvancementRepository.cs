using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.DAL.AdvancementAndRefund
{
    public interface IAdvancementRepository : IBaseRepository<Advancement>
    {
        bool Exist(int id);
        Advancement GetFullById(int id);
        //IList<Advancement> GetAllInProcess(int[] statesToExclude);
        IList<AdvancementHistory> GetHistories(int id);
        IList<Advancement> GetAllFinalized(AdvancementSearchFinalizedModel model, int workflowStatusDraft);
        IList<Advancement> GetByApplicant(int id);
        IList<Advancement> GetUnrelated(int currentUserId, int workflowStatusDraftId);
        IList<Advancement> GetAllPaymentPending(int workFlowStatePaymentPending);
        IList<AdvancementRefund> GetAdvancementRefundByRefundId(int entityId);
        void DeleteAdvancementRefund(AdvancementRefund advancementRefund);
        void UpdateStatus(Advancement advancement);
        Tuple<IList<Refund>, IList<Advancement>> GetAdvancementsAndRefundsByAdvancementId(IList<int> id);
        IList<Advancement> GetAllApproved(int workflowStatusApproveId, AdvancementType viaticumId);
        int GetRefundWithLastRefundMarkedCount(int advancementId, int refundId);
        IList<Advancement> GetSalaryResume(int settingsSalaryWorkflowId, int settingsWorkflowStatusApproveId);
        void AddSalaryDiscount(SalaryDiscount domain);
    }
}
