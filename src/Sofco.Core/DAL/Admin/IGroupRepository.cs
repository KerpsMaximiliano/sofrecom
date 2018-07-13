using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface IGroupRepository : IBaseRepository<Group>
    {
        Group GetSingleWithRole(Expression<Func<Group, bool>> predicate);

        Group GetSingleWithUser(Expression<Func<Group, bool>> predicate);

        Group GetSingleFull(Expression<Func<Group, bool>> predicate);

        bool ExistById(int id);

        IList<Group> GetAllFullReadOnly();

        IList<Group> GetAllActivesReadOnly();

        bool DescriptionExist(string description, int id);

        string GetEmail(string code);

        Group GetByCode(string guestCode);
    }
}
