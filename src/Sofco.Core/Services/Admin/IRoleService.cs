using System.Collections.Generic;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface IRoleService
    {
        IList<Role> GetAllReadOnly(bool active);
        Response<Role> GetById(int id);
        Response<Role> Insert(Role role);
        Response<Role> DeleteById(int id);
        Response<Role> Update(Role role);
        IList<Role> GetRolesByGroup(IEnumerable<int> groupIds);
        Response<Role> GetDetail(int id);
        Response<Role> Active(int id, bool active);
        Response<Role> AddFunctionalities(int roleId, List<int> functionalitiesToAdd);
        Response<Role> AddFunctionality(int roleId, int moduleId);
        Response<Role> DeleteFunctionality(int roleId, int moduleId);
        Response<Role> RemoveFunctionalities(int roleId, List<int> functionalities);
    }
}
