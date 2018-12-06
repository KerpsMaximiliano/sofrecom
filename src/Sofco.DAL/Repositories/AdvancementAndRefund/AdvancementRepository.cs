﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.Models;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;

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

        public Advancement GetById(int id)
        {
            return context.Advancements.SingleOrDefault(x => x.Id == id);
        }

        public Advancement GetFullById(int id)
        {
            return context.Advancements
                .Include(x => x.Currency)
                .Include(x => x.AdvancementReturnForm)
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
                .Include(x => x.Authorizer)
                .Include(x => x.Status)
                    .ThenInclude(x => x.ActualTransitions)
                        .ThenInclude(x => x.WorkflowStateAccesses)
                            .ThenInclude(x => x.UserSource)
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
    }
}
