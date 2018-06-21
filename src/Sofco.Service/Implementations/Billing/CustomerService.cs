﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Extensions;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.Logger;
using Sofco.Core.Models;
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
        private readonly ISessionManager sessionManager;
        private readonly ISolfacDelegateData solfacDelegateData;
        private readonly ILogMailer<CustomerService> logger;

        public CustomerService(ICustomerData customerData, 
            ICrmHttpClient client, 
            IOptions<CrmConfig> crmOptions, 
            ISessionManager sessionManager,
            ILogMailer<CustomerService> logger,
            ISolfacDelegateData solfacDelegateData)
        {
            this.customerData = customerData;
            this.client = client;
            this.sessionManager = sessionManager;
            this.solfacDelegateData = solfacDelegateData;
            crmConfig = crmOptions.Value;
            this.logger = logger;
        }

        public Response<List<CrmCustomer>> GetCustomers(bool getAll)
        {
            var response = new Response<List<CrmCustomer>>();
            var result = new List<CrmCustomer>();

            var userNames = solfacDelegateData.GetUserDelegateByUserName(sessionManager.GetUserName());
            foreach (var item in userNames)
            {
                try
                {
                    result.AddRange(customerData.GetCustomers(item, getAll).ToList());
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                    response.AddWarning(Resources.Common.CrmGeneralError);
                }
            }
            response.Data = result.DistinctBy(x => x.Id);

            return response;
        }

        public Response<List<SelectListModel>> GetCustomersOptions(bool getAll)
        {
            var result = GetCustomers(getAll);

            var response = new Response<List<SelectListModel>>
            {
                Data = result.Data
                    .Select(x => new SelectListModel {Id = x.Id, Text = x.Nombre})
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return response;
        }

        public Response<List<SelectListModel>> GetCustomersOptionsByCurrentManager()
        {
            var result = GetCustomersByCurrentManager();

            var response = new Response<List<SelectListModel>>
            {
                Data = result.Data
                    .Select(x => new SelectListModel { Id = x.Id, Text = x.Nombre })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return response;
        }

        public Response<CrmCustomer> GetCustomerById(string customerId)
        {
            var response = new Response<CrmCustomer>();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                response.AddError(Resources.Billing.Customer.NotFound);
                return response;
            }

            var url = $"{crmConfig.Url}/api/account/{customerId}";

            var customer = client.Get<CrmCustomer>(url).Data;

            if (customer.Id.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.AddError(Resources.Billing.Customer.NotFound);
                return response;
            }

            response.Data = customer;
            return response;
        }

        private Response<List<CrmCustomer>> GetCustomersByCurrentManager()
        {
            var response = new Response<List<CrmCustomer>>();

            var result = customerData.GetCustomersByManager(sessionManager.GetUserName()).ToList();

            response.Data = result.DistinctBy(x => x.Id);

            return response;
        }
    }
}
