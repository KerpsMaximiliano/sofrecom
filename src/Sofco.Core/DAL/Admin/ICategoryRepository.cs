using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        IList<Category> GetAllActives();
        Category GetById(int id);
        bool ExistById(int categoryId);
    }
}
