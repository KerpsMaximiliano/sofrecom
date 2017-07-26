using System;
using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Linq.Expressions;

namespace Sofco.Core.DAL
{
    public interface IFunctionalityRepository : IBaseRepository<Functionality>
    {
        bool ExistById(int id);
        Functionality GetSingleWithRoles(Expression<Func<Functionality, bool>> predicate);
    }
}
