using Sofco.Domain.Interfaces;

namespace Sofco.Domain.Models.RequestNote
{
    public class BuyOrderHistory : WorkflowHistory
    {
        public int BuyOrderId { get; set; }

        public BuyOrder BuyOrder { get; set; }

        public override void SetEntityId(int entityId)
        {
            this.BuyOrderId = entityId;
        }
    }
}
