using System;
using Sofco.Domain.Interfaces;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class AdvancementHistory : WorkflowHistory
    {
        public int AdvancementId { get; set; }

        public Advancement Advancement { get; set; }

        public override void SetEntityId(int entityId)
        {
            this.AdvancementId = entityId;
        }
    }
}
