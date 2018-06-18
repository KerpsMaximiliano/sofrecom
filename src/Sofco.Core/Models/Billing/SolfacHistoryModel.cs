using System;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class SolfacHistoryModel
    {
        public SolfacHistoryModel(SolfacHistory history)
        {
            CreatedDate = history.CreatedDate;

            Comment = history.Comment;
            SolfacStatusFrom = history.SolfacStatusFrom.ToString();
            SolfacStatusTo = history.SolfacStatusTo.ToString();

            if (history.User != null)
            {
                UserName = history.User.Name;
            }
        }

        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }

        public string Comment { get; set; }

        public string SolfacStatusFrom { get; set; }

        public string SolfacStatusTo { get; set; }
    }
}
