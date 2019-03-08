using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Refund;

namespace Sofco.Core.Models.AdvancementAndRefund.Common
{
    public class AdvancementRefundModel
    {
        public IList<RefundRelatedModel> Refunds { get; set; }

        public IList<AdvancementRelatedModel> Advancements { get; set; }
    }
}
