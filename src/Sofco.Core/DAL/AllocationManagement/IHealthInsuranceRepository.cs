using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IHealthInsuranceRepository : IBaseRepository<HealthInsurance>
    {
        void Save(List<HealthInsurance> healthInsurances);

        HealthInsurance GetByCode(int code);
    }
}