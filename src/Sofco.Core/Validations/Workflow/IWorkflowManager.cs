﻿using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.RequestNote;

namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowManager
    {
        void CloseAdvancementsAndRefunds(int entityId);
        void CloseAdvancements(IList<Advancement> advancements);
        void CloseRefunds(IList<Refund> refunds);
        void CloseEntity(WorkflowEntity entity);
        void PayRefunds(IList<Refund> refunds);
        int? CloseRequestNote(RequestNote note);
        int? PartialReceptionRequestNote(RequestNote note);
        int? PendingReceptionRequestNote(RequestNote note, BuyOrder order);
    }
}
