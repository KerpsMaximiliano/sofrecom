using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowManager
    {
        void CloseAdvancementsAndRefunds(int entityId);
        void CloseAdvancementsAndRefunds(Tuple<IList<Refund>, IList<Advancement>> data, int entityId);
        void CloseEntity(WorkflowEntity entity);
    }
}
