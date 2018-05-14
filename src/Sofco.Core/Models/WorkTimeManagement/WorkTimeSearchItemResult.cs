using System;
using Sofco.Model.Enums;

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
    }
}
