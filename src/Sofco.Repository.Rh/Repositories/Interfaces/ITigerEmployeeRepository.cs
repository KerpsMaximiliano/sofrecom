using Sofco.Domain.Rh.Tiger;
using System;
using System.Collections.Generic;

namespace Sofco.Repository.Rh.Repositories.Interfaces
{
    public interface ITigerEmployeeRepository
    {
        IList<TigerEmployee> GetWithStartDate(DateTime startDate);

        IList<TigerEmployee> GetWithEndDate(DateTime endDate);
    }
}
