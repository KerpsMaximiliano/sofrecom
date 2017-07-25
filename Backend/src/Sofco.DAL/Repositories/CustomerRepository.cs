using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;

namespace Sofco.DAL.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(SofcoContext context) : base(context)
        {
        }
    }
}
