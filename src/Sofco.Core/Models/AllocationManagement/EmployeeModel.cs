using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Models.AllocationManagement;

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
            Manager = domain.Manager?.Name;
            ManagerId = domain.Manager?.Id;
            AssignComments = domain.AssignComments;

            if (domain.EmployeeCategories != null && domain.EmployeeCategories.Any())
                Categories = domain.EmployeeCategories.Select(x => x.CategoryId).ToList();
            else
                Categories = new List<int>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public string Senority { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }

        public decimal Percentage { get; set; }

        public bool Selected { get; set; }

        public string Manager { get; set; }

        public int? ManagerId { get; set; }

        public DateTime StartDate { get; set; }

        public IList<int> Categories { get; set; }

        public string AssignComments { get; set; }

        public decimal PercentageAllocation { get; set; }
    }
}
