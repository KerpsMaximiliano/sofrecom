using System;
using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Sofco.Core.DAL
{
    public interface IUserRepository : IBaseRepository<User>
    {
        bool ExistById(int id);
        User GetSingleWithUserGroup(Expression<Func<User, bool>> predicate);
        IList<User> GetAllFullReadOnly();
    }
}
