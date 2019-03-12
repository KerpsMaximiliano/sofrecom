using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Repositories.AdvancementAndRefund
{
    public class RefundRepository : BaseRepository<Refund>, IRefundRepository
    {
        public RefundRepository(SofcoContext context) : base(context)
        {
        }

        public List<Refund> GetByParameters(RefundListParameterModel model)
        {
            var query = context.Refunds
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.Status)
                    .ThenInclude(x => x.ActualTransitions)
                .Include(x => x.Details)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Advancement)
                .Where(s => s.CreationDate.Date >= model.DateSince.Value.Date
                            && s.InWorkflowProcess == model.InWorkflowProcess);

            if (model.DateTo.HasValue)
                query = query.Where(x => x.CreationDate.Date <= model.DateTo.Value.Date);

            if (model.UserApplicantId.HasValue && model.UserApplicantId.Value > 0)
                query = query.Where(x => x.UserApplicantId == model.UserApplicantId.Value);

            if (model.StateId.HasValue)
            {
                query = query.Where(x => x.StatusId == model.StateId);
            }

            return query.ToList();
        }

        public void InsertFile(RefundFile refundFile)
        {
            context.RefundFiles.Add(refundFile);
        }

        public Refund GetFullById(int id)
        {
            return context.Refunds
                .Include(x => x.Currency)
                .Include(x => x.Analytic)
                .Include(x => x.UserApplicant)
                .Include(x => x.Status)
                .Include(x => x.CreditCard)
                .Include(x => x.Details)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Advancement)
                .Include(x => x.Attachments)
                    .ThenInclude(x => x.File)
                .SingleOrDefault(x => x.Id == id);
        }

        public RefundFile GetFile(int id, int fileId)
        {
            return context.RefundFiles.Include(x => x.File).SingleOrDefault(x => x.RefundId == id && x.FileId == fileId);
        }

        public void DeleteFile(RefundFile file)
        {
            context.RefundFiles.Remove(file);
        }

        public IList<RefundHistory> GetHistories(int id)
        {
            return context.RefundHistories
                .Include(x => x.StatusFrom)
                .Include(x => x.StatusTo)
                .Where(x => x.RefundId == id).ToList();
        }

        public bool Exist(int id)
        {
            return context.Refunds.Any(x => x.Id == id);
        }

        public Refund GetById(int id)
        {
            return context.Refunds.Include(x => x.Details)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Advancement)
                .SingleOrDefault(x => x.Id == id);
        }

        public IList<Refund> GetByApplicant(int id)
        {
            return context.Refunds
                .Include(x => x.Currency)
                .Include(x => x.Status)
                .Include(x => x.Details)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Advancement)
                .Where(x => x.UserApplicantId == id)
                .OrderByDescending(x => x.CreationDate).ToList();
        }

        public IList<Refund> GetAllPaymentPending(int workFlowStatePaymentPending)
        {
            var query = context.Refunds
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Advancement)
                .Include(x => x.Details)
                .Include(x => x.Status).ThenInclude(x => x.ActualTransitions)
                .Where(x => x.StatusId == workFlowStatePaymentPending);

            return query.ToList();
        }

        public Tuple<IList<Refund>, IList<Advancement>> GetAdvancementsAndRefundsByRefundId(int id)
        {
            var advancementIds = context.AdvancementRefunds
                .Where(x => x.RefundId == id)
                .Select(x => x.AdvancementId)
                .Distinct()
                .ToList();

            var advancements = context.Advancements
                .Where(x => advancementIds.Contains(x.Id))
                .ToList();

            var refundIds = context.AdvancementRefunds
                .Where(x => advancementIds.Contains(x.AdvancementId))
                .Select(x => x.RefundId)
                .Distinct()
                .ToList();

            var refunds = context.Refunds
                .Where(x => refundIds.Contains(x.Id) || x.Id == id)
                .Distinct()
                .ToList();

            return new Tuple<IList<Refund>, IList<Advancement>>(refunds, advancements);
        }

        public void UpdateStatus(Refund refund)
        {
            context.Entry(refund).Property("StatusId").IsModified = true;
        }

        public void UpdateCurrencyExchange(Refund refund)
        {
            context.Entry(refund).Property("CurrencyExchange").IsModified = true;
        }

        public IList<Refund> GetAllInCurrentAccount(int workflowStatusCurrentAccount)
        {
            return context.Refunds
                .Include(x => x.UserApplicant)
                .Include(x => x.Currency)
                .Where(x => x.StatusId == workflowStatusCurrentAccount)
                .ToList();
        }

        public bool HasAttachments(int entityId)
        {
            return context.RefundFiles.Any(x => x.RefundId == entityId);
        }
    }
}
