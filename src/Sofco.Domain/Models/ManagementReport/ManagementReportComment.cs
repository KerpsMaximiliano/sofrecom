using System;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ManagementReportComment : BaseEntity
    {
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }

        public int ManagementReportId { get; set; }

        public ManagementReport ManagementReport { get; set; }
    }
}
