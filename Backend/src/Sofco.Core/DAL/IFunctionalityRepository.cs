using System;
using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Sofco.Core.DAL
{
    public interface IFunctionalityRepository : IBaseRepository<Functionality>
    {
        bool ExistById(int id);
        Functionality GetSingleWithRoles(Expression<Func<Functionality, bool>> predicate);
        IList<Functionality> GetAllFullReadOnly();
    }
}
