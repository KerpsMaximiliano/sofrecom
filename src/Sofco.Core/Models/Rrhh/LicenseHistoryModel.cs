using System;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.Models.Rrhh
{
    public class LicenseHistoryModel
    {
        public LicenseHistoryModel(LicenseHistory history)
        {
            CreatedDate = history.CreatedDate;
            Comment = history.Comment;
            StatusFrom = history.LicenseStatusFrom.ToString();
            StatusTo = history.LicenseStatusTo.ToString();

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
