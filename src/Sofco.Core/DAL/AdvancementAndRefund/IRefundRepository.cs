using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.DAL.AdvancementAndRefund
{
    public interface IRefundRepository : IBaseRepository<Refund>
    {
        List<Refund> GetByParameters(RefundListParameterModel model, int workflowStatusDraft);

        void InsertFile(RefundFile refundFile);

        Refund GetFullById(int id);

        RefundFile GetFile(int id, int fileId);

        void DeleteFile(RefundFile file);

        IList<RefundHistory> GetHistories(int id);

        bool Exist(int id);

        Refund GetById(int id);

        IList<Refund> GetByApplicant(int id);

        IList<Refund> GetAllPaymentPending(int workFlowStatePaymentPending);

        Tuple<IList<Refund>, IList<Advancement>> GetAdvancementsAndRefundsByRefundId(int id);

        void UpdateStatus(Refund refund);

        void UpdateCurrencyExchange(Refund refund);

        IList<Refund> GetAllInCurrentAccount(int workflowStatusCurrentAccount);

        bool HasAttachments(int entityId);

        bool ExistAdvancementRefund(int advancement, int refund);

        void AddAdvancementRefund(AdvancementRefund advancementRefund);

        IList<RefundFile> GetFiles(int id);
        Refund GetFullByIdForZip(int id);

        IList<Refund> GetRefundsByAnalytics(int id);
    }
}
