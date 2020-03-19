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

        public List<Refund> GetByParameters(RefundListParameterModel model, int workflowStatusDraft)
        { 
            var query = context.Refunds
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.Analytic)
                    .ThenInclude(x => x.Manager)
                .Include(x => x.Status)
                    .ThenInclude(x => x.ActualTransitions)
                        .ThenInclude(x => x.WorkflowStateAccesses)
                            .ThenInclude(x => x.UserSource)
                .Include(x => x.Details)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Advancement)
                .Include(x => x.Histories)
                .Where(s => s.StatusId != workflowStatusDraft);

            if (model.DateSince.HasValue)
                query = query.Where(x => x.CreationDate.Date >= model.DateSince.Value.Date);

            if (model.DateTo.HasValue)
                query = query.Where(x => x.CreationDate.Date <= model.DateTo.Value.Date);

            if (model.UserApplicantId.HasValue && model.UserApplicantId.Value > 0)
                query = query.Where(x => x.UserApplicantId == model.UserApplicantId.Value);

            if (model.StateIds != null && model.StateIds.Any())
                query = query.Where(x => model.StateIds.Contains(x.StatusId));

            if (model.AnalyticIds != null && model.AnalyticIds.Any())
                query = query.Where(x => model.AnalyticIds.Contains(x.AnalyticId));

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
                    .ThenInclude(x => x.CostType)
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
                .Where(x => x.StatusId == workFlowStatePaymentPending && !x.CreditCardId.HasValue);

            return query.ToList();
        }

        public Tuple<IList<Refund>, IList<Advancement>> GetAdvancementsAndRefundsByRefundId(int id)
        {
            var advancementIds = context.AdvancementRefunds
                .Where(x => x.RefundId == id)
                .Select(x => x.AdvancementId)
                .Distinct()
                .ToList();

            var refundIds = context.AdvancementRefunds
                .Where(x => advancementIds.Contains(x.AdvancementId))
                .Select(x => x.RefundId)
                .Distinct()
                .ToList();

            var end = false;

            while (!end)
            {
                var advancementIdCount = advancementIds.Count;

                advancementIds = context.AdvancementRefunds
                    .Where(x => refundIds.Contains(x.RefundId))
                    .Select(x => x.AdvancementId)
                    .Distinct()
                    .ToList();

                if (advancementIdCount < advancementIds.Count)
                {
                    refundIds = context.AdvancementRefunds
                        .Where(x => advancementIds.Contains(x.AdvancementId))
                        .Select(x => x.RefundId)
                        .Distinct()
                        .ToList();
                }
                else
                {
                    end = true;
                }
            }

            var advancements = context.Advancements
                .Where(x => advancementIds.Contains(x.Id))
                .ToList();

            var refunds = context.AdvancementRefunds
                .Where(x => advancementIds.Contains(x.AdvancementId))
                .Include(x => x.Refund)
                .Select(x => x.Refund)
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
                .Include(x => x.AdvancementRefunds)
                .Where(x => x.StatusId == workflowStatusCurrentAccount && !x.CreditCardId.HasValue)
                .ToList();
        }

        public bool HasAttachments(int entityId)
        {
            return context.RefundFiles.Any(x => x.RefundId == entityId);
        }

        public bool ExistAdvancementRefund(int advancement, int refund)
        {
            return context.AdvancementRefunds.Any(x => x.AdvancementId == advancement && x.RefundId == refund);
        }

        public void AddAdvancementRefund(AdvancementRefund advancementRefund)
        {
            context.AdvancementRefunds.Add(advancementRefund);
        }

        public IList<RefundFile> GetFiles(int id)
        {
            return context.RefundFiles.Where(x => x.RefundId == id).ToList();
        }

        public Refund GetFullByIdForZip(int id)
        {
            return context.Refunds
                .Include(x => x.Currency)
                .Include(x => x.Analytic)
                .Include(x => x.UserApplicant)
                .Include(x => x.Status)
                .Include(x => x.CreditCard)
                .Include(x => x.Details)
                    .ThenInclude(x => x.CostType)
                .Include(x => x.AdvancementRefunds)
                .Include(x => x.Histories)
                    .ThenInclude(x => x.StatusFrom)
                .Include(x => x.Histories)
                    .ThenInclude(x => x.StatusTo)
                .SingleOrDefault(x => x.Id == id);
        }
    }
}
