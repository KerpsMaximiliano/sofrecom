using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(SofcoContext context) : base(context)
        {
        }

        public Customer GetByIdCrm(string crmCustomerId)
        {
            return context.Customers.SingleOrDefault(x => x.CrmId.Equals(crmCustomerId));
        }

        public IList<Customer> GetAllByServices(IEnumerable<string> servicesIds)
        {
            return context.Customers.Where(x => servicesIds.Contains(x.CrmId)).ToList().AsReadOnly();
        }

        public IList<Customer> GetAllActives()
        {
            return context.Customers.Where(x => x.Active).ToList().AsReadOnly();
        }
    }
}
