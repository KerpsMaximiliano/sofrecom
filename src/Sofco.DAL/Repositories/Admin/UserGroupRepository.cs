using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories.Admin
{
    public class UserGroupRepository : BaseRepository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int userId, int groupId)
        {
            return context.UserGroup.Any(x => x.UserId == userId && x.GroupId == groupId);
        }

        public int[] GetGroupsId(string userName)
        {
            return context.UserGroup
                .Include(x => x.User)
                .Include(x => x.Group)
                .Where(x => x.User.UserName.Equals(userName) && x.Group.Active)
                .Select(x => x.GroupId)
                .Distinct()
                .ToArray();
        }
    }
}
