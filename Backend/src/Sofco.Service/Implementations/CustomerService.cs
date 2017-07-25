using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;

namespace Sofco.Service.Implementations
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository, IBaseRepository<Customer> repoBase) : base(repoBase)
        {
            _repository = repository;
        }

        public void DeleteById(int id)
        {
            var entity = _repository.GetSingle(x => x.Id == id);

            if(entity != null)
            {
                _repository.Delete(entity);
                _repository.Save(currentUser);
            }
        }

        public Customer GetById(int id)
        {
            return _repository.GetSingle(x => x.Id == id);
        }
    }
}
