using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Interfaces.Services
{
    public interface IUserGroupService
    {
        IList<UserGroup> GetAllReadOnly();
        Response<UserGroup> GetById(int id);
        Response<UserGroup> Insert(UserGroup role);
        Response<UserGroup> DeleteById(int id);
        Response<UserGroup> Update(UserGroup role);
        Response<UserGroup> AddRole(int roleId, int userGroupId);
    }
}
