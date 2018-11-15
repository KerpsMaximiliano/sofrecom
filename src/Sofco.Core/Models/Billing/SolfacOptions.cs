using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.Models.Billing
{
    public class SolfacOptions
    {
        public IList<Option> Provinces { get; set; }

        public IList<Option> DocumentTypes { get; set; }

        public IList<Option> ImputationNumbers { get; set; }

        public IList<Option> Currencies { get; set; }

        public IList<Option> PaymentTerms { get; set; }

        public IList<ListItem<string>> PurchaseOrders { get; set; }
    }
}
