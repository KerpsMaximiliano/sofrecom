
using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Core.Services.Admin
{
    public interface IFunctionalityService
    {
        IList<Functionality> GetAllReadOnly(bool active);
        Response<Functionality> GetById(int id);
        Response<Functionality> Active(int id, bool active);
        IList<Functionality> GetFunctionalitiesByModule(int moduleId);
        IList<Functionality> GetFunctionalitiesByModule(IEnumerable<int> modules);
        IList<RoleFunctionality> GetFunctionalitiesByRole(IEnumerable<int> roles);
    }
}
