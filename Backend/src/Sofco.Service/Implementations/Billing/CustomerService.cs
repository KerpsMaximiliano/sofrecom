using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;

namespace Sofco.Service.Implementations.Billing
{
    public class CustomerService : ICustomerService
    {
        ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository) 
        {
            _repository = repository;
        }
    }
}
