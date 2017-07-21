using Sofco.Model.Models;

namespace Sofco.Core.Interfaces.Services
{
    public interface ICustomerService : IBaseService<Customer>
    {
        void DeleteById(int id);

        Customer GetById(int id);
    }
}
