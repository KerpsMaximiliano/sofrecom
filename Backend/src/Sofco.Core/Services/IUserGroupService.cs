using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Interfaces.Services
{
    public interface IUserGroupService : IBaseService<UserGroup>
    {
        void DeleteById(int id);

        UserGroup GetById(int id);

        Response<UserGroup> AddRole(int roleId, int userGroupId);

        UserGroup GetByIdWithRole(int id);

        IList<UserGroup> GetAllReadOnlyWithEntitiesRelated();
    }
}
