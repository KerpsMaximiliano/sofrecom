using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        Service GetByIdCrm(string crmServiceId);
        IList<Service> GetAllActives(string customerId);
        IList<Service> GetAllByManager(string customerId, string externalManagerId);
        void UpdateActive(Service service);
        void UpdateAnalytic(Service service);
        IList<Service> GetAllNotRelatedOptions(string customerId);
        IList<Service> GetAllByManager(string externalManagerId);
    }
}
