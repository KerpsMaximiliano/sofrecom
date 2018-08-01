using System.Collections.Generic;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface IModuleService
    {
        IList<Module> GetAllReadOnly(bool active);
        Response<Module> GetById(int id);
        Response<Module> Active(int id, bool active);
        Response<Module> Update(Module data);
        IList<Module> GetAllWithFunctionalitiesReadOnly();
    }
}
