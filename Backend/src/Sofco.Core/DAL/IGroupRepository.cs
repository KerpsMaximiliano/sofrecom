using System;
using Sofco.Model.Models;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Sofco.Core.Interfaces.DAL
{
    public interface IGroupRepository : IBaseRepository<Group>
    {
        Group GetSingleWithRole(Expression<Func<Group, bool>> predicate);
        Group GetSingleWithUser(Expression<Func<Group, bool>> predicate);
        Group GetSingleFull(Expression<Func<Group, bool>> predicate);
        bool ExistById(int id);
        IList<Group> GetAllFullReadOnly();
        IList<Group> GetAllActivesReadOnly();
    }
}
