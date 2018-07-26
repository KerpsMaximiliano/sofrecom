using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Billing;

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

        public IList<Customer> GetAllActives()
        {
            return context.Customers.Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<Customer> GetAllByManager(string externalManagerId)
        {
            return context.Customers.Where(x => x.OwnerId.Equals(externalManagerId) && x.Active).ToList();
        }
    }
}
