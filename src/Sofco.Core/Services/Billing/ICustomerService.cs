using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ICustomerService
    {
        Response<List<Customer>> GetCustomers();

        Response<List<SelectListModel>> GetCustomersOptions();

        Response<List<SelectListModel>> GetCustomersOptionsByCurrentManager();

        Response<Customer> GetCustomerById(string customerId);

        Response Update();

        Response<IList<SelectListModel>> GetAllCustomersOptions();
    }
}
