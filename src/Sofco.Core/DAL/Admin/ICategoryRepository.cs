using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        IList<Category> GetAllActives();
        Category GetById(int id);
        bool ExistById(int categoryId);
        bool DescriptionExist(string description);
        bool ExistEmployeeCategory(int employeeId, int categoryId);
        void AddEmployeeCategory(EmployeeCategory employeeCategory);
    }
}
