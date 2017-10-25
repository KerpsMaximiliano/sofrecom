using Sofco.Model.Models.TimeManagement;
using System;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class AllocationModel
    {
        public AllocationModel(Allocation allocation)
        {
            AnalyticId = allocation.AnalyticId;
            EmployeeId = allocation.EmployeeId;
            StartDate = allocation.StartDate;
            EndDate = allocation.EndDate;
            Percentage = allocation.Percentage;

            if(allocation.Analytic != null)
            {
                AnalyticTitle = allocation.Analytic.Title;
                AnalyticName = allocation.Analytic.Name;
                ClientName = allocation.Analytic.ClientExternalName;
            }
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Percentage { get; set; }
        public int AnalyticId { get; set; }
        public int EmployeeId { get; set; }
        public string AnalyticTitle { get; set; }
        public string AnalyticName { get; set; }
        public string ClientName { get; set; }
    }
}
