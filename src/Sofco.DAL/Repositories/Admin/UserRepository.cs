using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Models.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private const string ManagerDescription = "Gerentes";

        private readonly EmailConfig emailConfig;

        public UserRepository(SofcoContext context, IOptions<EmailConfig> emailConfig) : base(context)
        {
            this.emailConfig = emailConfig.Value;
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
                    .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Description.Equals("Directores")));
        }

        public IList<User> GetDirectors()
        {
            var userGroups = context.UserGroup
                .Include(x => x.Group)
                .Include(x => x.User)
                .Where(x => x.Group.Description.Equals("Directores"))
                .ToList();

            return userGroups.Select(x => x.User).ToList();
        }

        public IList<User> GetManagers()
        {
            var userGroups = context.UserGroup
                .Include(x => x.Group)
                .Include(x => x.User)
                .Where(x => x.Group.Description.Equals(ManagerDescription))
                .ToList();

            return userGroups.Select(x => x.User).ToList();
        }

        public IList<User> GetSellers()
        {
            var sellerCode = emailConfig.SellerCode;

            var userGroups = context.UserGroup
                .Include(x => x.Group)
                .Include(x => x.User)
                .Where(x => x.Group.Code.Equals(sellerCode))
                .ToList();

            return userGroups.Select(x => x.User).ToList();
        }

        public bool HasComercialGroup(string email)
        {
            var comercialCode = emailConfig.ComercialCode;
            return context.Users
                .Include(x => x.UserGroups)
                    .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(email) && x.UserGroups.Any(s => s.Group.Code.Equals(comercialCode)));
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

        public bool HasDafGroup(string userMail)
        {
            var dafCode = emailConfig.DafCode;

            return context.Users
                .Include(x => x.UserGroups)
                    .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == dafCode));
        }

        public bool HasManagersGroup(string userMail)
        {
            var managerCode = emailConfig.ManagersCode;

            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == managerCode));
        }

        public bool HasCdgGroup(string userMail)
        {
            var cdgCode = emailConfig.CdgCode;

            return context.Users
                 .Include(x => x.UserGroups)
                    .ThenInclude(x => x.Group)
                 .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == cdgCode));
        }

        public bool IsActive(string userMail)
        {
            return context.Users.Any(x => x.Email == userMail && x.Active);
        }

        public bool HasManagerGroup(string userName)
        {
            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => 
                x.UserName.Equals(userName) 
                && x.UserGroups.Any(s => s.Group.Description.Equals(ManagerDescription)));
        }

        public bool HasRrhhGroup(string userMail)
        {
            var rrhhCode = emailConfig.RrhhCode;

            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == rrhhCode));
        }

        public UserLiteModel GetUserLiteById(int userId)
        {
            return context.Users.Where(s => s.Id == userId).Select(s => new UserLiteModel
            {
                Id = s.Id,
                Name = s.Name
            }).FirstOrDefault();
        }
    }
}
