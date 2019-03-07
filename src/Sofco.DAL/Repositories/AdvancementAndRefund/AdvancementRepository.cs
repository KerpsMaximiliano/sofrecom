using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Repositories.AdvancementAndRefund
{
    public class AdvancementRepository : BaseRepository<Advancement>, IAdvancementRepository
    {
        public AdvancementRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int id)
        {
            return context.Advancements.Any(x => x.Id == id);
        }

        public Advancement GetFullById(int id)
        {
            return context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.MonthsReturn)
                .Include(x => x.UserApplicant)
                .Include(x => x.Status)
                .SingleOrDefault(x => x.Id == id);
        }

        public IList<Advancement> GetAllInProcess()
        {
            return context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.Authorizer)
                .Include(x => x.MonthsReturn)
                .Include(x => x.Status)
                    .ThenInclude(x => x.ActualTransitions)
                        .ThenInclude(x => x.WorkflowStateAccesses)
                            .ThenInclude(x => x.UserSource)
                .Where(x => x.InWorkflowProcess).ToList();
        }

        public IList<WorkflowReadAccess> GetWorkflowReadAccess(int advacementWorkflowId)
        {
            return context.WorkflowReadAccesses
                .Include(x => x.Workflow)
                .Include(x => x.UserSource)
                .Where(x => x.Workflow.WorkflowTypeId == advacementWorkflowId)
                .ToList();
        }

        public IList<AdvancementHistory> GetHistories(int id)
        {
            return context.AdvancementHistories
                .Include(x => x.StatusFrom)
                .Include(x => x.StatusTo)
                .Where(x => x.AdvancementId == id).ToList();
        }

        public IList<Advancement> GetAllFinalized(int statusDraft, AdvancementSearchFinalizedModel model)
        {
            var query = context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.MonthsReturn)
                .Include(x => x.Authorizer)
                .Include(x => x.Status)
                .Where(x => !x.InWorkflowProcess && x.StatusId != statusDraft);

            if (model.ResourceId.HasValue && model.ResourceId.Value > 0)
                query = query.Where(x => x.UserApplicantId == model.ResourceId.Value);

            if (model.TypeId.HasValue && model.TypeId.Value > 0)
                query = query.Where(x => x.Type == (AdvancementType) model.TypeId.Value);

            if (model.DateSince.HasValue)
                query = query.Where(x => x.CreationDate.Date >= model.DateSince.Value.Date);

            if (model.DateTo.HasValue)
                query = query.Where(x => x.CreationDate.Date <= model.DateTo.Value.Date);

            return query.ToList();
        }

        public IList<Advancement> GetByApplicant(int id)
        {
            return context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.Status)
                .Where(x => x.UserApplicantId == id)
                .OrderByDescending(x => x.CreationDate).ToList();
        }

        public IList<Advancement> GetUnrelated(int currentUserId, int workflowStatusOpenId)
        {

            var advancements = context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Refund)
                .Where(x => x.UserApplicantId == currentUserId  && x.Type == AdvancementType.Viaticum && x.StatusId == workflowStatusOpenId)
                .ToList();

            return advancements;
        }

        public IList<Advancement> GetAllPaymentPending(int workFlowStatePaymentPending)
        {
            var query = context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.MonthsReturn)
                .Include(x => x.Authorizer)
                .Include(x => x.Status).ThenInclude(x => x.ActualTransitions)
                .Where(x => x.StatusId == workFlowStatePaymentPending);

            return query.ToList();
        }

        public IList<AdvancementRefund> GetAdvancementRefundByRefundId(int entityId)
        {
            return context.AdvancementRefunds.Include(x => x.Advancement).Where(x => x.RefundId == entityId).ToList();
        }

        public void DeleteAdvancementRefund(AdvancementRefund advancementRefund)
        {
            context.AdvancementRefunds.Remove(advancementRefund);
        }

        public void UpdateStatus(Advancement advancement)
        {
            context.Entry(advancement).Property("StatusId").IsModified = true;
        }

        public Tuple<IList<Refund>, IList<Advancement>> GetAdvancementsAndRefundsByAdvancementId(int id)
        {
            var refundIds = context.AdvancementRefunds
                .Where(x => x.AdvancementId == id)
                .Select(x => x.RefundId)
                .Distinct()
                .ToList();

            var refunds = context.Refunds
                .Include(x => x.Status)
                .Include(x => x.Analytic)
                .Where(x => refundIds.Contains(x.Id))
                .ToList();

            var advancementIds = context.AdvancementRefunds
                .Where(x => refundIds.Contains(x.RefundId))
                .Select(x => x.AdvancementId)
                .Distinct()
                .ToList();

            var advancements = context.Advancements
                .Where(x => advancementIds.Contains(x.Id))
                .ToList();

            return new Tuple<IList<Refund>, IList<Advancement>>(refunds, advancements);
        }

        public IList<Advancement> GetAllApproved(int workflowStatusApproveId)
        {
            return context.Advancements.Where(x => x.StatusId == workflowStatusApproveId).ToList();
        }

        public int GetRefundWithLastRefundMarkedCount(int advancementId, int refundId)
        {
            return context.AdvancementRefunds
                .Include(x => x.Refund)
                .Where(x => x.AdvancementId == advancementId && x.RefundId != refundId)
                .Count(x => x.Refund.CashReturn);
        }
    }
}
