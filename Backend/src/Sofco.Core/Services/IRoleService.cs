using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Interfaces.Services
{
    public interface IRoleService
    {
        IList<Role> GetAllFullReadOnly();
        IList<Role> GetAllReadOnly();
        Response<Role> GetById(int id);
        Response<Role> Insert(Role role);
        Response<Role> DeleteById(int id);
        Response<Role> Update(Role role);
        Response<Role> ChangeFunctionalities(int roleId, int moduleId, List<int> functionlitiesToAdd, List<int> functionlitiesToRemove);
        Response<Role> ChangeMenus(int roleId, List<int> menusToAdd, List<int> menusToRemove);
        IList<Role> GetRolesByGroup(IEnumerable<int> groupIds);
        Response<Functionality> AddFunctionality(int roleId, int moduleId, int functionalityId);
        Response<Functionality> DeleteFunctionality(int roleId, int moduleId, int functionalityId);
        Response<Role> GetDetail(int id);
        Response<Role> Active(int id, bool active);
    }
}
