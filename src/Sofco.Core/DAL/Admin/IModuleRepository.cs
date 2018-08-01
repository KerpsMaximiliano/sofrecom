using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models;
using Sofco.Domain.Models.Admin;

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
