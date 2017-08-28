using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public GroupRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return _context.Set<Group>().Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<Group> GetAllActivesReadOnly()
        {
            return _context.Set<Group>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<Group> GetAllFullReadOnly()
        {
            return _context.Set<Group>()
                .Include(x => x.Role)
                .Include(x => x.UserGroups)
                    .ThenInclude(s => s.User)
                .ToList();
        }

        public Group GetSingleFull(Expression<Func<Group, bool>> predicate)
        {
            return _context.Set<Group>()
                .Include(x => x.Role)
                .Include(x => x.UserGroups)
                   .ThenInclude(s => s.User)
                .SingleOrDefault(predicate);
        }

        public Group GetSingleWithRole(Expression<Func<Group, bool>> predicate)
        {
            return _context.Set<Group>()
                .Include(x => x.Role)
                .SingleOrDefault(predicate);
        }

        public Group GetSingleWithUser(Expression<Func<Group, bool>> predicate)
        {
            return _context.Set<Group>()
              .Include(x => x.UserGroups)
                    .ThenInclude(s => s.User)
              .SingleOrDefault(predicate);
        }
    }
}
