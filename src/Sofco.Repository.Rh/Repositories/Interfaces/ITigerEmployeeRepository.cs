using Sofco.Domain.Rh.Tiger;
using System;
using System.Collections.Generic;

namespace Sofco.Repository.Rh.Repositories.Interfaces
{
    public interface ITigerEmployeeRepository
    {
        IList<TigerEmployee> GetWithStartDate(DateTime startDate);

        IList<TigerEmployee> GetWithEndDate(DateTime endDate);

        List<TigerEmployee> GetByLegajs(int[] legajs);

        List<TigerEmployee> GetActive();

        IList<EmployeeSocialCharges> GetSocialCharges(int year, int month);
    }
}
