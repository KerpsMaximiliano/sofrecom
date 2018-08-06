using System.Collections.Generic;

namespace Sofco.Core.Models.Billing
{
    public class InvoiceAnnulmentModel
    {
        public IList<int> Invoices { get; set; }

        public string Comments { get; set; }
    }
}
