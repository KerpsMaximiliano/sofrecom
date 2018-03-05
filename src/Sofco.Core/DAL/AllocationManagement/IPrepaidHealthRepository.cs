using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IPrepaidHealthRepository
    {
        void Save(List<PrepaidHealth> prepaidHealths);

        PrepaidHealth GetByCode(int healthInsuranceCode, int prepaidHealthCode);
    }
}