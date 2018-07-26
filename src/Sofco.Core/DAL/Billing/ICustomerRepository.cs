using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Customer GetByIdCrm(string crmCustomerId);
        IList<Customer> GetAllActives();
        IList<Customer> GetAllByManager(string externalManagerId);
    }
}
