using System;
using System.Collections.Generic;

namespace Sofco.Model.DTO
{
    public class AllocationDto
    {
        public AllocationDto()
        {
            this.Months = new List<AllocationMonthDto>();
        }

        public int AnalyticId { get; set; }

        public string AnalyticTitle { get; set; }

        public int EmployeeId { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public IList<AllocationMonthDto> Months { get; set; }

        public int Id { get; set; }
    }

    public class AllocationMonthDto
    {
        public int AllocationId { get; set; }

        public DateTime Date { get; set; }

        public decimal Percentage { get; set; }

        public bool Updated { get; set; }

        public DateTime ReleaseDate { get; set; }
    }

    public class AllocationResponse
    {
        public AllocationResponse()
        {
            this.MonthsHeader = new List<MonthHeader>();
            this.Allocations = new List<AllocationDto>();
        }

        public IList<AllocationDto> Allocations { get; set; }

        public IList<MonthHeader> MonthsHeader { get; set; }
    }

    public class MonthHeader
    {
        public string Display { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
