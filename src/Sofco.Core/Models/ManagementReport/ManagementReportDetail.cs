using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.ManagementReport
{
    public class ManagementReportDetail
    {
        public string Analytic { get; set; }

        public string ServiceType { get; set; }

        public string SolutionType { get; set; }

        public string TechnologyType { get; set; }

        public string Manager { get; set; }
        public int? ManagerId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IList<string> Opportunities { get; set; }

        public IList<string> PurchaseOrders { get; set; }

        public IList<AmmountItem> Ammounts { get; set; }

        public string Name { get; set; }

        public string AccountName { get; set; }

        public int  ManagementReportId { get; set; }

        public DateTime ManamementReportStartDate { get; set; }

        public DateTime ManamementReportEndDate { get; set; }

        public AnalyticStatus AnalyticStatus { get; set; }
    }

    public class AmmountItem
    {
        public string Currency { get; set; }

        public decimal Value { get; set; }
    }
}
