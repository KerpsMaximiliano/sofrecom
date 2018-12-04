using System.Collections.Generic;

namespace Sofco.Domain.DTO
{
    public class EmployeeAddCategoriesParams
    {
        public IList<int> CategoriesToAdd { get; set; }
        public IList<int> CategoriesToRemove { get; set; }
        public IList<int> Employees { get; set; }
    }
}
