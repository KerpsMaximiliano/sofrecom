using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Admin;

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
    }
}
