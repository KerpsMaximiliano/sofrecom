using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
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
            return context.Set<User>().Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<User> GetAllActivesReadOnly()
        {
            return context.Set<User>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public bool HasDirectorGroup(string userMail)
        {
            return context.Users
                .Include(x => x.UserGroups)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Description.Equals("Directores")));
        }

        public Group GetGroup(int userId)
        {
            return context.UserGroup
                .Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.Group)
                .Select(x => x.Group)
                .SingleOrDefault();
        }

        public IList<User> GetAllFullReadOnly()
        {
            return context.Set<User>()
                .Include(x => x.UserGroups)
                    .ThenInclude(s => s.Group)
                .ToList()
                .AsReadOnly();
        }

        public User GetSingleWithUserGroup(Expression<Func<User, bool>> predicate)
        {
            return context.Set<User>()
              .Include(x => x.UserGroups)
                    .ThenInclude(s => s.Group)
              .SingleOrDefault(predicate);
        }

        public bool ExistByMail(string mail)
        {
            return context.Users.Any(x => x.Email == mail);
        }

        public bool HasDafGroup(string userMail, string dafCode)
        {
            return context.Users
                .Include(x => x.UserGroups)
                    .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == dafCode));
        }

        public bool HasCdgGroup(string userMail, string cdgCode)
        {
            return context.Users
                 .Include(x => x.UserGroups)
                    .ThenInclude(x => x.Group)
                 .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == cdgCode));
        }
    }
}
