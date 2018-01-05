using System.Collections.Generic;

namespace Sofco.Model.Utils
{
    public class AnalyticOptions
    {
        public IList<Option> Activities { get; set; }
        public IList<Option> Directors { get; set; }
        public IList<Option> Managers { get; set; }
        public IList<Option> Currencies { get; set; }
        public IList<Option> Solutions { get; set; }
        public IList<Option> Technologies { get; set; }
        public IList<Option> Products { get; set; }
        public IList<Option> ClientGroups { get; set; }
        public IList<Option> PurchaseOrders { get; set; }
        public IList<Option> SoftwareLaws { get; set; }
        public IList<Option> ServiceTypes { get; set; }
        public IList<Option> Sellers { get; set; }
    }
}
