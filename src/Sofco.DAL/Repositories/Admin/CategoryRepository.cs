using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Repositories.Admin
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Category> GetAllActives()
        {
            return context.Categories.Where(x => x.Active).ToList().AsReadOnly();
        }

        public Category GetById(int id)
        {
            return context.Categories.Include(x => x.Tasks).SingleOrDefault(x => x.Id == id);
        }

        public bool ExistById(int categoryId)
        {
            return context.Categories.Any(x => x.Id == categoryId);
        }

        public bool DescriptionExist(string description)
        {
            return context.Categories.Any(x => x.Description == description);
        }

        public bool ExistEmployeeCategory(int employeeId, int categoryId)
        {
            return context.EmployeeCategories.Any(x => x.CategoryId == categoryId && x.EmployeeId == employeeId);
        }

        public void AddEmployeeCategory(EmployeeCategory employeeCategory)
        {
            context.EmployeeCategories.AddRange(employeeCategory);
        }
    }
}
