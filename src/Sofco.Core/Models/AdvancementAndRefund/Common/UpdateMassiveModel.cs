using System.Collections.Generic;

namespace Sofco.Core.Models.AdvancementAndRefund.Common
{
    public class UpdateMassiveModel
    {
        public IList<int> Advancements { get; set; }
        public IList<int> Refunds { get; set; }
    }
}
