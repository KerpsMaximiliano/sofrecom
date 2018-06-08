using System;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeModel
    {
        public EmployeeModel()
        {
        }

        public EmployeeModel(Employee domain)
        {
            Id = domain.Id;
            Name = domain.Name;
            EmployeeNumber = domain.EmployeeNumber;
            Senority = domain.Seniority;
            Profile = domain.Profile;
            Technology = domain.Technology;
            Percentage = domain.BillingPercentage;
            StartDate = domain.StartDate;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public string Senority { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }

        public decimal Percentage { get; set; }

        public bool Selected { get; set; }

        public DateTime StartDate { get; set; }
    }
}
