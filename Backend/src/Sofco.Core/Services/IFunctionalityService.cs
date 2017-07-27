
using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services
{
    public interface IFunctionalityService
    {
        IList<Functionality> GetAllFullReadOnly();
        IList<Functionality> GetAllReadOnly();
        Response<Functionality> GetById(int id);
        Response<Functionality> Active(int id, bool active);
    }
}
