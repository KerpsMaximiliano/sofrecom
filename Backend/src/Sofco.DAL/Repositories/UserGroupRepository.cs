using Sofco.Core.DAL;
using Sofco.Model.Relationships;
using System.Linq;

namespace Sofco.DAL.Repositories
{
    public class UserGroupRepository : BaseRepository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int userId, int groupId)
        {
            return _context.UserGroup.Any(x => x.UserId == userId && x.GroupId == groupId);
        }
    }
}
