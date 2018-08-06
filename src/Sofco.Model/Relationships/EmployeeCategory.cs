using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Relationships
{
    public class EmployeeCategory
    {
        public EmployeeCategory()
        {
            
        }

        public EmployeeCategory(int employeeId, int categoryId)
        {
            EmployeeId = employeeId;
            CategoryId = categoryId;
        }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
