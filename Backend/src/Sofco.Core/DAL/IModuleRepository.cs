using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sofco.Core.DAL
{
    public interface IModuleRepository : IBaseRepository<Module>
    {
        bool ExistById(int id);
        Module GetSingleWithFunctionalities(Expression<Func<Module, bool>> predicate);
        IList<Module> GetAllActivesReadOnly();
    }
}
