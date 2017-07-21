using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;

namespace Sofco.DAL.Repositories
{
    public class UserGroupRepository : BaseRepository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(SofcoContext context) : base(context)
        {
        }
    }
}
