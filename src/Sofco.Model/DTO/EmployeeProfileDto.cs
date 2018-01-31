using System;
using System.Collections.Generic;

namespace Sofco.Model.DTO
{
    public class EmployeeProfileDto
    {
        public EmployeeProfileDto()
        {
            this.Allocations = new List<EmployeeAllocationDto>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public string Seniority { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }

        public decimal Percentage { get; set; }

        public string Office { get; set; }

        public string Manager { get; set; }

        public IList<EmployeeAllocationDto> Allocations { get; set; }
    }

    public class EmployeeAllocationDto
    {
        public string Title { get; set; }

        public string Name { get; set; }

        public string Client { get; set; }

        public string Service { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ReleaseDate { get; set; }
    }
}
