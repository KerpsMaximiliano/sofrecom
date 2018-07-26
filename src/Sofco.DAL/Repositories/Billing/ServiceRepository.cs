using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(SofcoContext context) : base(context)
        {
        }

        public Service GetByIdCrm(string crmServiceId)
        {
            return context.Services.SingleOrDefault(x => x.CrmId.Equals(crmServiceId));
        }

        public IList<Service> GetAllActives(string customerId)
        {
            return context.Services.Where(x => x.AccountId.Equals(customerId) && x.Active).ToList().AsReadOnly();
        }

        public IList<Service> GetAllByManager(string customerId, string externalManagerId)
        {
            return context.Services.Where(x => x.Active && x.AccountId.Equals(customerId) && x.ManagerId.Equals(externalManagerId)).ToList().AsReadOnly();
        }
    }
}
