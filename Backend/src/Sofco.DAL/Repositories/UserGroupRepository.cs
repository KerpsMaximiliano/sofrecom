using System;
using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Sofco.DAL.Repositories
{
    public class UserGroupRepository : BaseRepository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(SofcoContext context) : base(context)
        {
        }

        public IList<UserGroup> GetAllReadOnlyWithEntitiesRelated()
        {
            return _context.Set<UserGroup>()
                .Include(x => x.Role)
                .ToList();
        }

        public UserGroup GetSingleWithRole(Expression<Func<UserGroup, bool>> predicate)
        {
            return _context.Set<UserGroup>()
                .Include("Role")
                .SingleOrDefault(predicate);
        }
    }
}
