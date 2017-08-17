using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Sofco.Core.Interfaces.DAL;
using System.Linq.Expressions;

namespace Sofco.DAL.Repositories
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
