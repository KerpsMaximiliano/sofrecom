using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface ICategoryService
    {
        Response Add(string description);
        IList<Category> GetAll(bool active);
        Response<CategoryModel> GetById(int id);
        Response<Category> Active(int id, bool active);
        Response Update(CategoryModel model);
    }
}
