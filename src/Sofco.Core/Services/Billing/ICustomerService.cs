using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ICustomerService
    {
        Response<List<CrmCustomer>> GetCustomers();

        Response<List<SelectListModel>> GetCustomersOptions();

        Response<List<SelectListModel>> GetCustomersOptionsByCurrentManager();

        Response<CrmCustomer> GetCustomerById(string customerId);
    }
}
