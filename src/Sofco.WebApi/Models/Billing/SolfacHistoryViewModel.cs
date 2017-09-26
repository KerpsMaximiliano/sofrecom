using System;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacHistoryViewModel
    {
        public SolfacHistoryViewModel(SolfacHistory history)
        {
            CreatedDate = history.CreatedDate;
            UserName = history.User.UserName;
            Comment = history.Comment;
            SolfacStatusFrom = history.SolfacStatusFrom.ToString();
            SolfacStatusTo = history.SolfacStatusTo.ToString();
        }

        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public string SolfacStatusFrom { get; set; }
        public string SolfacStatusTo { get; set; }
    }
}
