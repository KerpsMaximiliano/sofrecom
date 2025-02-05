﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;

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

        //public IList<Advancement> GetAllInProcess(int[] statesToExclude)
        //{
        //    return context.Advancements
        //        .Include(x => x.Currency)
        //        .Include(x => x.UserApplicant)
        //        .Include(x => x.MonthsReturn)
        //        .Include(x => x.Status)
        //            .ThenInclude(x => x.ActualTransitions)
        //                .ThenInclude(x => x.WorkflowStateAccesses)
        //                    .ThenInclude(x => x.UserSource)
        //        .Where(x => !statesToExclude.Contains(x.StatusId)).ToList();
        //}

        public IList<AdvancementHistory> GetHistories(int id)
        {
            return context.AdvancementHistories
                .Include(x => x.StatusFrom)
                .Include(x => x.StatusTo)
                .Where(x => x.AdvancementId == id).ToList();
        }

        public IList<Advancement> GetAllFinalized(AdvancementSearchFinalizedModel model, int workflowStatusDraft)
        {
            var query = context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.MonthsReturn)
                .Include(x => x.Histories)
                .Include(x => x.Status)
                    .ThenInclude(x => x.ActualTransitions)
                        .ThenInclude(x => x.WorkflowStateAccesses)
                            .ThenInclude(x => x.UserSource)
                .Where(x => x.StatusId != workflowStatusDraft);

            if (model.StateIds != null && model.StateIds.Any())
                query = query.Where(x => model.StateIds.Contains(x.StatusId));

            if (model.ResourceId.HasValue && model.ResourceId.Value > 0)
                query = query.Where(x => x.UserApplicantId == model.ResourceId.Value);

            if (model.TypeId.HasValue && model.TypeId.Value > 0)
                query = query.Where(x => x.Type == (AdvancementType)model.TypeId.Value);

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
                .Where(x => x.UserApplicantId == currentUserId && x.Type == AdvancementType.Viaticum && x.StatusId == workflowStatusOpenId)
                .ToList();

            return advancements;
        }

        public IList<Advancement> GetAllPaymentPending(int workFlowStatePaymentPending)
        {
            var query = context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.MonthsReturn)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Refund)
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

        public Tuple<IList<Refund>, IList<Advancement>> GetAdvancementsAndRefundsByAdvancementId(IList<int> ids)
        {
            var refundIds = context.AdvancementRefunds
                .Where(x => ids.Contains(x.AdvancementId))
                .Select(x => x.RefundId)
                .Distinct()
                .ToList();

            var advancementIds = context.AdvancementRefunds
                .Where(x => refundIds.Contains(x.RefundId))
                .Select(x => x.AdvancementId)
                .Distinct()
                .ToList();
          
            var end = false;

            while (!end)
            {
                var refundIdCount = refundIds.Count;

                refundIds = context.AdvancementRefunds
                    .Where(x => advancementIds.Contains(x.AdvancementId))
                    .Select(x => x.RefundId)
                    .Distinct()
                    .ToList();

                if (refundIdCount < refundIds.Count)
                {
                    advancementIds = context.AdvancementRefunds
                        .Where(x => refundIds.Contains(x.RefundId))
                        .Select(x => x.AdvancementId)
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
                .Include(x => x.Status)
                .Include(x => x.Analytic)
                .Distinct()
                .ToList();
            
            return new Tuple<IList<Refund>, IList<Advancement>>(refunds, advancements);
        }

        public IList<Advancement> GetAllApproved(int workflowStatusApproveId, AdvancementType viaticumId)
        {
            return context.Advancements
                .Include(x => x.UserApplicant)
                .Include(x => x.Currency)
                .Where(x => x.StatusId == workflowStatusApproveId && x.Type == viaticumId).ToList();
        }

        public int GetRefundWithLastRefundMarkedCount(int advancementId, int refundId)
        {
            return context.AdvancementRefunds
                .Include(x => x.Refund)
                .Where(x => x.AdvancementId == advancementId && x.RefundId != refundId)
                .Count(x => x.Refund.LastRefund);
        }

        public IList<Advancement> GetSalaryResume(int salaryWorkflowId, int workflowStatusApproveId)
        {
            return context.Advancements
                .Include(x => x.UserApplicant)
                .Where(x =>x.Type == (AdvancementType) salaryWorkflowId && x.StatusId == workflowStatusApproveId)
                .ToList();
        }

        public void AddSalaryDiscount(SalaryDiscount domain)
        {
            context.AdvancementSalaryDiscounts.Add(domain);
        }

        public Advancement GetFullByIdForZip(int id)
        {
            return context.Advancements
                .Include(x => x.MonthsReturn)
                .Include(x => x.UserApplicant)
                .Include(x => x.AdvancementRefunds)
                .Include(x => x.Currency)
                .Include(x => x.Histories)
                    .ThenInclude(x => x.StatusFrom)
                .Include(x => x.Histories)
                    .ThenInclude(x => x.StatusTo)
                .SingleOrDefault(x => x.Id == id);
        }
    }
}
