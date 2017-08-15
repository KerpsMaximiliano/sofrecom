using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL
{
    public interface IUserGroupRepository : IBaseRepository<UserGroup>
    {
        bool ExistById(int userId, int groupId);
        int[] GetGroupsId(int userId);
    }
}
