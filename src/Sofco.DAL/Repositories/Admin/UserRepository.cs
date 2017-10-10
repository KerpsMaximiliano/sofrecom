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

        public bool HasDirectorGroup(string userMail)
        {
            return _context.Users
                .Include(x => x.UserGroups)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Description.Equals("Directores")));
        }

        public Group GetGroup(int userId)
        {
            return _context.UserGroup
                .Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.Group)
                .Select(x => x.Group)
                .SingleOrDefault();
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

        public bool ExistByMail(string mail)
        {
            return _context.Users.Any(x => x.Email == mail);
        }
    }
}
