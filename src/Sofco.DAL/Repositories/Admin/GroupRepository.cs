using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.Admin
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        private readonly EmailConfig appSetting;

        public GroupRepository(SofcoContext context, IOptions<EmailConfig> appSettingOptions) : base(context)
        {
            this.appSetting = appSettingOptions.Value;
        }

        public bool DescriptionExist(string description, int id)
        {
            return context.Groups.Any(x => x.Description == description && x.Id != id);
        }

        public bool ExistById(int id)
        {
            return context.Set<Group>().Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<Group> GetAllActivesReadOnly()
        {
            return context.Set<Group>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<Group> GetAllFullReadOnly()
        {
            return context.Set<Group>()
                .Include(x => x.Role)
                .Include(x => x.UserGroups)
                    .ThenInclude(s => s.User)
                .ToList();
        }

        public Group GetSingleFull(Expression<Func<Group, bool>> predicate)
        {
            return context.Set<Group>()
                .Include(x => x.Role)
                .Include(x => x.UserGroups)
                   .ThenInclude(s => s.User)
                .SingleOrDefault(predicate);
        }

        public Group GetSingleWithRole(Expression<Func<Group, bool>> predicate)
        {
            return context.Set<Group>()
                .Include(x => x.Role)
                .SingleOrDefault(predicate);
        }

        public string GetEmail(string code)
        {
            return context.Groups
                .Where(s => s.Code == code)
                .Select(s => s.Email)
                .FirstOrDefault();
        }

        public Group GetByCode(string guestCode)
        {
            return context.Groups.SingleOrDefault(x => x.Code.Equals(guestCode));
        }

        public IList<Group> GetByUsers(IEnumerable<int> userids)
        {
            var users = context.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .Where(x => userids.Contains(x.Id))
                .ToList();

            var groups = new List<Group>();

            foreach (var user in users)
            {
                foreach (var userGroup in user.UserGroups)
                {
                    if (groups.All(x => x.Id != userGroup.GroupId))
                    {
                        groups.Add(userGroup.Group);
                    }
                }
            }

            return groups;
        }

        public bool IsManagerOrDirector(Employee employee)
        {
            return context.UserGroup.Any(x =>
                (x.Group.Code == appSetting.ManagersCode ||  x.Group.Code == appSetting.DirectorsCode)
                && x.User.Email == employee.Email);
        }
    }
}
