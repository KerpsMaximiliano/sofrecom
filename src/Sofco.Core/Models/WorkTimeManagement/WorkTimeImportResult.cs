using System;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeImportResult
    {
        public string EmployeeNumber { get; set; }
        public string Employee { get; set; }
        public string Row { get; set; }
        public string Date { get; set; }
        public string Error { get; set; }
    }
}
