using System;

namespace Sofco.Core.Models.ManagementReport
{
    public class MarginTrackingModel
    {
        public decimal PercentageExpected { get; set; }

        public decimal PercentageRealToDate { get; set; }

        public decimal PercentageToEnd { get; set; }

        public decimal ExpectedSales { get; set; }

        public decimal TotalExpensesExpected { get; set; }

        public decimal SalesOnMonth { get; set; }

        public decimal TotalExpensesOnMonth { get; set; }

        public decimal SalesAccumulatedToDate { get; set; }

        public decimal TotalExpensesAccumulatedToDate { get; set; }

        public decimal SalesRemainigToDate { get; set; }

        public decimal TotalExpensesRemainigToDate { get; set; }

        public decimal TotalSalesToEnd { get; set; }

        public decimal TotalExpensesToEnd { get; set; }

        public DateTime Date { get; set; }
    }
}
