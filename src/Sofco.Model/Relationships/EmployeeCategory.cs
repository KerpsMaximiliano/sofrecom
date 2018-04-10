using Sofco.Model.Models.Admin;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Model.Relationships
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
