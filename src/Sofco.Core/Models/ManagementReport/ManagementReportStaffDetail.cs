using System;
using System.Collections.Generic;
using Sofco.Core.Models.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.Core.Models.ManagementReport
{
    public class ManagementReportStaffDetail
    {
        public string Analytic { get; set; }

        public string Sector { get; set; }

        public string Manager { get; set; }

        public int? ManagerId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IList<BudgetItem> Budgets { get; set; }

        public IList<BudgetItem> BudgetsHistory { get; set; }

        public int  ManagementReportId { get; set; }

        public DateTime ManamementReportStartDate { get; set; }

        public DateTime ManamementReportEndDate { get; set; }

        public ManagementReportStatus Status { get; set; }

        public decimal BudgetTotal { get; set; }

        public int AnalyticId { get; set; }

        public int DelegateId { get; set; }

        public IList<CurrencyExchangeModel> Months { get; set; }

        public bool StateGenerated { get; set; }
    }

    public class BudgetItem
    {
        public BudgetItem()
        {
        }

        public BudgetItem(Budget domain)
        {
            Id = domain.Id;
            Description = domain.Description;
            Value = domain.Value;
            LastValue = domain.LastValue;
            StartDate = domain.StartDate;
            ModifiedBy = domain.ModifiedBy;
        }

        public decimal LastValue { get; set; }

        public int Id { get; set; }

        public string Description { get; set; }

        public string ModifiedBy { get; set; }

        public decimal Value { get; set; }

        public DateTime StartDate { get; set; }
    }

    public class BudgetTypeItem
    {
        public BudgetTypeItem()
        {
        }

        public BudgetTypeItem(BudgetType domain)
        {
            Id = domain.Id;
            Name = domain.Name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
