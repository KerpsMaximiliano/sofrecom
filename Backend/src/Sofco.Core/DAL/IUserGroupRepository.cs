using System;
using Sofco.Model.Models;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Sofco.Core.Interfaces.DAL
{
    public interface IUserGroupRepository : IBaseRepository<UserGroup>
    {
        UserGroup GetSingleWithRole(Expression<Func<UserGroup, bool>> p);
        IList<UserGroup> GetAllReadOnlyWithEntitiesRelated();
    }
}
