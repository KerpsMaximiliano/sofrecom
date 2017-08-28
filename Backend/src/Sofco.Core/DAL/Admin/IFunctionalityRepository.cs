using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface IFunctionalityRepository : IBaseRepository<Functionality>
    {
        bool ExistById(int id);
        IList<Functionality> GetAllActivesReadOnly();
    }
}
