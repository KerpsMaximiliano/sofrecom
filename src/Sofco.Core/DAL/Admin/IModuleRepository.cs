using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface IModuleRepository : IBaseRepository<Module>
    {
        bool ExistById(int id);
        Module GetSingleWithFunctionalities(Expression<Func<Module, bool>> predicate);
        IList<Module> GetAllActivesReadOnly();
        IList<Module> GetAllWithFunctionalitiesReadOnly();
    }
}
