using System.Collections.Generic;
using Sofco.Domain.Rh.Tiger;

namespace Sofco.Repository.Rh.Repositories.Interfaces
{
    public interface ITigerHealthInsuranceRepository
    {
        List<TigerHealthInsurance> GetAll();
    }
}