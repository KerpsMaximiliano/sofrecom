using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services
{
    public interface IModuleService
    {
        IList<Module> GetAllFullReadOnly();
        IList<Module> GetAllReadOnly(bool active);
        Response<Module> GetById(int id);
        Response<Module> Active(int id, bool active);
        Response<Module> Update(Module data);
    }
}
