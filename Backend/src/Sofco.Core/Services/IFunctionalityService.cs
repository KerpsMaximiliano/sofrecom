
using Sofco.Model.Models;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services
{
    public interface IFunctionalityService
    {
        IList<Functionality> GetAllReadOnly(bool active);
        Response<Functionality> GetById(int id);
        Response<Functionality> Active(int id, bool active);
        IList<Functionality> GetFunctionalitiesByModule(int moduleId);
        IList<Functionality> GetFunctionalitiesByModule(IEnumerable<int> modules);
    }
}
