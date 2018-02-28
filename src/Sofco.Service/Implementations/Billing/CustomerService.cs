using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Implementations.Billing
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerData customerData;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;

        public CustomerService(ICustomerData customerData, ICrmHttpClient client, IOptions<CrmConfig> crmOptions)
        {
            this.customerData = customerData;
            this.client = client;
            crmConfig = crmOptions.Value;
        }

        public IList<CrmCustomer> GetCustomers(string userName)
        {
            return customerData.GetCustomers(userName);
        }

        public Response<CrmCustomer> GetCustomerById(string customerId)
        {
            var url = $"{crmConfig.Url}/api/account/{customerId}";

            var customer = client.Get<CrmCustomer>(url).Data;

            var response = new Response<CrmCustomer>();

            if (customer.Id.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.AddError(Resources.Billing.Customer.NotFound);
                return response;
            }

            response.Data = customer;
            return response;
        }
    }
}
