using System;
using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Collections.Generic;

namespace Sofco.Core.DAL
{
    public interface IFunctionalityRepository : IBaseRepository<Functionality>
    {
        bool ExistById(int id);
        IList<Functionality> GetAllActivesReadOnly();
    }
}
