using System;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class InvoiceHistoryModel
    {
        public InvoiceHistoryModel(InvoiceHistory history)
        {
            CreatedDate = history.CreatedDate;
            Comment = history.Comment;
            StatusFrom = history.StatusFrom.ToString();
            StatusTo = history.StatusTo.ToString();

            if (history.User != null)
                UserName = history.User.Name;
        }

        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }

        public string Comment { get; set; }

        public string StatusFrom { get; set; }

        public string StatusTo { get; set; }
    }
}
