using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Services.Admin
{
    public interface IModuleService
    {
        IList<Module> GetAllReadOnly(bool active);
        Response<Module> GetById(int id);
        Response<Module> Active(int id, bool active);
        Response<Module> Update(Module data);
        Response<Module> ChangeFunctionalities(int moduleId, List<int> functionlitiesToAdd);
        Response<Functionality> AddFunctionality(int moduleId, int functionalityId);
        Response<Functionality> DeleteFunctionality(int moduleId, int functionalityId);
        IList<Module> GetModulesByRole(IEnumerable<int> enumerable);
    }
}
