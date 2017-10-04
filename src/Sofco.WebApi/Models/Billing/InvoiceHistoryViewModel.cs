using System;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class InvoiceHistoryViewModel
    {
        public InvoiceHistoryViewModel(InvoiceHistory history)
        {
            CreatedDate = history.CreatedDate;
            UserName = history.User.UserName;
            Comment = history.Comment;
            StatusFrom = history.StatusFrom.ToString();
            StatusTo = history.StatusTo.ToString();
        }

        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public string StatusFrom { get; set; }
        public string StatusTo { get; set; }
    }
}
