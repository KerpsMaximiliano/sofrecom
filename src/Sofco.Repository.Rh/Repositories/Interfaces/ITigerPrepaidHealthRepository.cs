using System.Collections.Generic;
using Sofco.Domain.Rh.Tiger;

namespace Sofco.Repository.Rh.Repositories.Interfaces
{
    public interface ITigerPrepaidHealthRepository
    {
        List<TigerPrepaidHealth> GetAll();
    }
}