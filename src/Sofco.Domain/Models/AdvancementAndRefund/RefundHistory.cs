using Sofco.Domain.Interfaces;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class RefundHistory : WorkflowHistory
    {
        public int RefundId { get; set; }

        public Refund Refund { get; set; }

        public override void SetEntityId(int entityId)
        {
            this.RefundId = entityId;
        }
    }
}
