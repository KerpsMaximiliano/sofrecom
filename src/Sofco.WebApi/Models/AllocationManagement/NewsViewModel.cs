using System;
using Newtonsoft.Json;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class NewsViewModel
    {
        public NewsViewModel(EmployeeSyncAction domain)
        {
            Id = domain.Id;
            EmployeeNumber = domain.EmployeeNumber;
            Status = domain.Status;
            StartDate = domain.StartDate;
            EndDate = domain.EndDate;

            var employeedata = JsonConvert.DeserializeObject<Employee>(domain.EmployeeData);
            Name = employeedata.Name;
        }

        public int Id { get; set; }

        public string EmployeeNumber { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        public string EmployeeData { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
