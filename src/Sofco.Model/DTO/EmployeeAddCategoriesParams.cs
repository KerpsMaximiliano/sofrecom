using System.Collections.Generic;

namespace Sofco.Model.DTO
{
    public class EmployeeAddCategoriesParams
    {
        public IList<int> Categories { get; set; }
        public IList<int> Employees { get; set; }
    }
}
