using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models;

namespace Sofco.DAL.Repositories.Billing
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(SofcoContext context) : base(context)
        {
        }
    }
}
