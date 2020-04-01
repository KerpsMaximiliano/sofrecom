using System;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeSearchItemResult
    {
        public string Client { get; set; }

        public string Analytic { get; set; }

        public string Manager { get; set; }

        public string Employee { get; set; }

        public string Profile { get; set; }

        public string Category { get; set; }

        public string Task { get; set; }

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        public string Status { get; set; }

        public string Reference { get; set; }

        public string Comments { get; set; }

        public string AnalyticTitle { get; set; }

        public int Id { get; set; }

        public bool CanDelete { get; set; }
        public int StatusId { get; set; }
        public int AnalyticId { get; set; }
    }
}
