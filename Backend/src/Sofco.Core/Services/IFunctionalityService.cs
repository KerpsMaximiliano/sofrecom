
using Sofco.Model.Models;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services
{
    public interface IFunctionalityService
    {
        IList<Functionality> GetAllFullReadOnly();
        IList<Functionality> GetAllReadOnly(bool active);
        Response<Functionality> GetById(int id);
        Response<Functionality> Active(int id, bool active);
        IList<RoleModuleFunctionality> GetFunctionalitiesByRole(IEnumerable<int> roleIds);
        IList<Functionality> GetFunctionalitiesByModuleAndRole(int moduleId, int roleId);
    }
}
