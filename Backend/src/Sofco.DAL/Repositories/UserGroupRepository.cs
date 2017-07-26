using Sofco.Core.DAL;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories
{
    public class UserGroupRepository : BaseRepository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(SofcoContext context) : base(context)
        {
        }
    }
}
