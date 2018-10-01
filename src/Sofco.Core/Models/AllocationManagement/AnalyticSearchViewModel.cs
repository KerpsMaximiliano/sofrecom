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
            ClientExternalName = domain.ClientExternalName;
            ServiceName = domain.Service;
            StartDate = domain.StartDateContract;
            EndDate = domain.EndDateContract;
            Status = domain.Status;

            if (domain.SoftwareLaw != null)
            {
                SoftwareLaw = domain.SoftwareLaw.Text;
            }
        }

        public int Id { get; }

        public string Name { get; }

        public string Title { get; }

        public string ServiceName { get; }

        public string SoftwareLaw { get; }

        public AnalyticStatus Status { get; set; }

        public string ClientExternalName { get; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
