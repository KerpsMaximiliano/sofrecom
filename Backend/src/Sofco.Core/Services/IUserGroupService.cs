using Sofco.Model.Models;

namespace Sofco.Core.Interfaces.Services
{
    public interface IUserGroupService : IBaseService<UserGroup>
    {
        void DeleteById(int id);

        UserGroup GetById(int id);
    }
}
