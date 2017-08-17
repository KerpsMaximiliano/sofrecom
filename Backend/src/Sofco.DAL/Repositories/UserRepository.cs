using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL;
using Sofco.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace Sofco.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return _context.Set<User>().Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<User> GetAllActivesReadOnly()
        {
            return _context.Set<User>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<User> GetAllFullReadOnly()
        {
            return _context.Set<User>()
                .Include(x => x.UserGroups)
                    .ThenInclude(s => s.Group)
                .ToList()
                .AsReadOnly();
        }

        public User GetSingleWithUserGroup(Expression<Func<User, bool>> predicate)
        {
            return _context.Set<User>()
              .Include(x => x.UserGroups)
                    .ThenInclude(s => s.Group)
              .SingleOrDefault(predicate);
        }
    }
}
