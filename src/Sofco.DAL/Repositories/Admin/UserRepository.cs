using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sofco.Common.Extensions;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Models.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly EmailConfig emailConfig;

        private readonly AppSetting appSetting;

        public UserRepository(SofcoContext context, IOptions<EmailConfig> emailConfig, IOptions<AppSetting> appSettingOptions) : base(context)
        {
            this.emailConfig = emailConfig.Value;
            this.appSetting = appSettingOptions.Value;
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
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == emailConfig.DirectorsCode));
        }

        public IList<User> GetDirectors()
        {
            var userGroups = context.UserGroup
                .Include(x => x.Group)
                .Include(x => x.User)
                .Where(x => x.Group.Code == emailConfig.DirectorsCode && x.User.Active)
                .ToList();

            return userGroups.Select(x => x.User).ToList();
        }

        public IList<User> GetManagers()
        {
            var userGroups = context.UserGroup
                .Include(x => x.Group)
                .Include(x => x.User)
                .Where(x => x.Group.Code == emailConfig.ManagersCode || x.Group.Code == emailConfig.DirectorsCode && x.User.Active)
                .ToList();

            return userGroups.Select(x => x.User).ToList().DistinctBy(s => s.Name);
        }

        public IList<User> GetCommercialManagers()
        {
            var result = context.Analytics
                .Include(x => x.CommercialManager)
                .Where(x => x.CommercialManagerId > 0 && x.CommercialManager.Active)
                .Select(x => x.CommercialManager)
                .Distinct();

            return result.ToList();
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

        public bool HasPmoGroup(string userMail)
        {
            var pmoCode = emailConfig.PmoCode;

            return context.Users
                .Include(x => x.UserGroups)
                    .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == pmoCode));
        }

        public bool HasManagersGroup(string userMail)
        {
            var managerCode = emailConfig.ManagersCode;

            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == managerCode));
        }

        public UserLiteModel GetUserLiteByUserName(string userName)
        {
            return context.Users.Where(s => s.UserName == userName).Select(s => new UserLiteModel
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                UserName = userName,
                ManagerId = s.ExternalManagerId
            }).FirstOrDefault();
        }

        public User GetByEmail(string email)
        {
            return context.Users.SingleOrDefault(x => x.Email == email);
        }

        public IList<User> GetAuthorizers()
        {
            var userGroups = context.UserGroup
                .Include(x => x.Group)
                .Include(x => x.User)
                .Where(x => x.Group.Code == emailConfig.ManagersCode || x.Group.Code == emailConfig.DirectorsCode || x.Group.Code == emailConfig.AuthCode)
                .ToList();

            return userGroups.Select(x => x.User).Distinct().ToList();
        }

        public bool HasComplianceGroup(string userEmail)
        {
            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userEmail) && x.UserGroups.Any(s => s.Group.Code == emailConfig.ComplianceCode));
        }

        public IList<User> GetByGroup(string groupCode)
        {
            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Where(x => x.UserGroups.Any(s => s.Group.Code == groupCode))
                .ToList();
        }

        public bool HasDafPurchaseOrderGroup(string userMail)
        {
            var dafCode = appSetting.DafPurchaseOrderGroupCode;

            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(userMail) && x.UserGroups.Any(s => s.Group.Code == dafCode));
        }

        public IList<User> GetExternalsFree()
        {
            var mails = context.Employees.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(x => x.Email).ToList();

            return context.Users.Where(x => !mails.Contains(x.Email)).ToList();
        }

        public List<UserLiteModel> GetUserLiteByIds(List<int> userIds)
        {
            return context.Users.Where(s => userIds.Contains(s.Id))
                .Select(s => new UserLiteModel { Id = s.Id, Name = s.Name})
                .ToList();
        }

        public bool HasGafGroup(string email)
        {
            var gafCode = emailConfig.GafCode;

            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(email) && x.UserGroups.Any(s => s.Group.Code == gafCode));
        }

        public List<UserLiteModel> GetUserLiteByEmails(List<string> emails)
        {
            return context.Users
                .Where(x => emails.Contains(x.Email))
                .Select(s => new UserLiteModel {Id = s.Id, Name = s.Name})
                .ToList();
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
                && x.UserGroups.Any(s => s.Group.Code == emailConfig.ManagersCode));
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
                Name = s.Name,
                Email = s.Email
            }).FirstOrDefault();
        }

        public bool HasComercialGroup(string comercialCode, string email)
        {
            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(email) && x.UserGroups.Any(s => s.Group.Code.Equals(comercialCode)));
        }
        
        public bool HasReadOnlyGroup(string currentUserEmail)
        {
            var readOnlyCode = emailConfig.ReadOnlyCode;

            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(currentUserEmail) && x.UserGroups.Any(s => s.Group.Code == readOnlyCode));
        }

        public IList<User> GetByIdsWithGroups(IEnumerable<int> ids)
        {
            var users = context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Where(x => ids.Contains(x.Id))
                .ToList();

            return users;
        }

        public bool HasSensibleDataGroup(string currentUserEmail)
        {
            return context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Any(x => x.Email.Equals(currentUserEmail) && x.UserGroups.Any(s => s.Group.Code == appSetting.SensibleFunctionality));
        }
    }
}
