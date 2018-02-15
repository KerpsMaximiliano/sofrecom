using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Implementations.Billing
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICustomerData customerData;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;
        private readonly EmailConfig emailConfig;

        public CustomerService(IUnitOfWork unitOfWork, ICustomerData customerData, ICrmHttpClient client, IOptions<CrmConfig> crmOptions, IOptions<EmailConfig> emailConfig)
        {
            this.unitOfWork = unitOfWork;
            this.customerData = customerData;
            this.client = client;
            this.crmConfig = crmOptions.Value;
            this.emailConfig = emailConfig.Value;
        }

        public IList<CrmCustomer> GetCustomers(string userMail, string identityName)
        {
            var hasDirectorGroup = this.unitOfWork.UserRepository.HasDirectorGroup(userMail);
            var hasCommercialGroup = this.unitOfWork.UserRepository.HasComercialGroup(emailConfig.ComercialCode, userMail);

            return customerData.GetCustomers(identityName, userMail, hasDirectorGroup || hasCommercialGroup);
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
