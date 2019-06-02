using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public GroupRepository(SofcoContext context) : base(context)
        {
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
    }
}
