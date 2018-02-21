using System.Collections.Generic;

namespace Sofco.Model.Utils
{
    public class PurchaseOrderOptions
    {
        public IList<Option> Analytics { get; set; }
        public IList<Option> Managers { get; set; }
        public IList<Option> Sellers { get; set; }
    }
}
