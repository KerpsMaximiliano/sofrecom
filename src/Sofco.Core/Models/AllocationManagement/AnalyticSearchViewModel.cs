using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class AnalyticSearchViewModel
    {
        public AnalyticSearchViewModel()
        {
        }

        public AnalyticSearchViewModel(Analytic domain)
        {
            Id = domain.Id;
            Title = domain.Title;
            Name = domain.Name;
            ClientExternalName = domain.AccountName;
            ClientId = domain.AccountId;
            ServiceName = domain.ServiceName;
            ServiceId = domain.ServiceId;
            StartDate = domain.StartDateContract;
            EndDate = domain.EndDateContract;
            Status = domain.Status;
            ManagerId = domain.ManagerId;
            SectorId = domain.SectorId;

            if (domain.ManagementReport != null && string.IsNullOrWhiteSpace(domain.ServiceId))
            {
                ManagementReportId = domain.ManagementReport.Id;
            }

            if (domain.Activity != null)
            {
                Activity = domain.Activity.Text;
            }
        }

        public int Id { get; }

        public string Name { get; }

        public string Title { get; }

        public string ServiceName { get; }

        public string Activity { get; }

        public AnalyticStatus Status { get; set; }

        public string ClientExternalName { get; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ServiceId { get; set; }

        public string ClientId { get; set; }

        public int? ManagerId { get; set; }

        public int ManagementReportId { get; set; }

        public int SectorId { get; set; }
    }
}
