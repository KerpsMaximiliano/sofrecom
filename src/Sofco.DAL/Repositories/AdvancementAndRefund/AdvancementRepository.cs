﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.DAL.Repositories.Common;
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
    }
}
