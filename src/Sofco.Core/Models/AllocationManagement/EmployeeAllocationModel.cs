using System;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeAllocationModel
    {
        public string Title { get; set; }

        public string Name { get; set; }

        public string Client { get; set; }

        public string Service { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ReleaseDate { get; set; }
    }
}
