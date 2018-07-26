using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ICustomerService
    {
        Response<List<Customer>> GetCustomers();

        Response<List<SelectListModel>> GetCustomersOptions();

        Response<List<SelectListModel>> GetCustomersOptionsByCurrentManager();

        Response<Customer> GetCustomerById(string customerId);
    }
}
