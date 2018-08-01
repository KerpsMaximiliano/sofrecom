﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerData customerData;
        private readonly ISessionManager sessionManager;
        private readonly ISolfacDelegateData solfacDelegateData;
        private readonly ILogMailer<CustomerService> logger;
        private readonly IUnitOfWork unitOfWork;

        public CustomerService(ICustomerData customerData, 
            ISessionManager sessionManager,
            ILogMailer<CustomerService> logger,
            IUnitOfWork unitOfWork,
            ISolfacDelegateData solfacDelegateData)
        {
            this.customerData = customerData;
            this.sessionManager = sessionManager;
            this.solfacDelegateData = solfacDelegateData;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response<List<Customer>> GetCustomers()
        {
            var response = new Response<List<Customer>> { Data = new List<Customer>() };

            var userNames = solfacDelegateData.GetUserDelegateByUserName(sessionManager.GetUserName());

            foreach (var item in userNames)
            {
                try
                {
                    response.Data.AddRange(customerData.GetCustomers(item));
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                    response.AddWarning(Resources.Common.CrmGeneralError);
                }
            }

            return response;
        }

        public Response<List<SelectListModel>> GetCustomersOptions()
        {
            var result = GetCustomers();

            var response = new Response<List<SelectListModel>>
            {
                Data = result.Data
                    .Select(x => new SelectListModel {Id = x.CrmId, Text = x.Name})
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
                    .Select(x => new SelectListModel { Id = x.CrmId, Text = x.Name })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return response;
        }

        public Response<Customer> GetCustomerById(string customerId)
        {
            var response = new Response<Customer>();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                response.AddError(Resources.Billing.Customer.NotFound);
                return response;
            }

            var customer = unitOfWork.CustomerRepository.GetByIdCrm(customerId);

            if (customer == null)
            {
                response.AddError(Resources.Billing.Customer.NotFound);
                return response;
            }

            response.Data = customer;
            return response;
        }

        private Response<IList<Customer>> GetCustomersByCurrentManager()
        {
            var response = new Response<IList<Customer>>();

            response.Data = customerData.GetCustomers(sessionManager.GetUserName());

            return response;
        }
    }
}
