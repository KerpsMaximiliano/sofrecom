using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Interfaces.Services
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
        Response<Role> ChangeModules(int roleId, List<int> modulesToAdd);
        Response<Role> AddModule(int roleId, int moduleId);
        Response<Role> DeleteModule(int roleId, int moduleId);
    }
}
