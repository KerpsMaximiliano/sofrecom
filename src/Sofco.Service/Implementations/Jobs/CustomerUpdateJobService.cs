using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm.Billing;
using Sofco.Domain.Models.Billing;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class CustomerUpdateJobService : ICustomerUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;
        private readonly ILogMailer<CustomerUpdateJobService> logger;
        private readonly ICustomerData customerData;
        private readonly ICrmAccountService crmAccountService;

        private IList<int> IdsAdded { get; set; }

        public CustomerUpdateJobService(IUnitOfWork unitOfWork, 
            ICrmHttpClient client, 
            ICustomerData customerData,
            IOptions<CrmConfig> crmOptions, 
            ILogMailer<CustomerUpdateJobService> logger, 
            ICrmAccountService crmAccountService)
        {
            this.unitOfWork = unitOfWork;
            this.client = client;
            crmConfig = crmOptions.Value;
            this.logger = logger;
            this.crmAccountService = crmAccountService;
            this.customerData = customerData;

            IdsAdded = new List<int>();
        }

        public void Execute()
        {
            var list = crmAccountService.GetById(new Guid("93bec8b8-24e1-e811-a953-000d3ac06054"));

            var url = $"{crmConfig.Url}/api/account";

            var result = client.GetMany<CrmCustomer>(url);

            unitOfWork.BeginTransaction();

            foreach (var crmCustomer in result.Data)
            {
                var customer = unitOfWork.CustomerRepository.GetByIdCrm(crmCustomer.Id);

                if (customer == null)
                    Create(crmCustomer);
                else
                    Update(crmCustomer, customer);
            }

            try
            {
                unitOfWork.CustomerRepository.UpdateInactives(IdsAdded);

                unitOfWork.Commit();
                customerData.ClearKeys();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void Update(CrmCustomer crmCustomer, Customer customer)
        {
            FillData(customer, crmCustomer);

            try
            {
                unitOfWork.CustomerRepository.Update(customer);
                unitOfWork.Save();

                IdsAdded.Add(customer.Id);
            }
            catch (Exception e)
            {
                logger.LogError($"Error modificacion cliente: {crmCustomer.Nombre}", e);
            }
        }

        private void Create(CrmCustomer crmCustomer)
        {
            var customer = new Customer();

            FillData(customer, crmCustomer);

            try
            {
                unitOfWork.CustomerRepository.Insert(customer);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error alta cliente: {crmCustomer.Nombre}", e);
            }
        }

        private void FillData(Customer customer, CrmCustomer crmCustomer)
        {
            customer.CrmId = crmCustomer.Id;
            customer.Address = crmCustomer.Address;
            customer.Cuit = crmCustomer.CUIT;
            customer.City = crmCustomer.City;
            customer.Contact = crmCustomer.Contact;
            customer.Country = crmCustomer.Country;
            customer.CurrencyDescription = crmCustomer.CurrencyDescription;
            customer.CurrencyId = crmCustomer.CurrencyId;
            customer.Name = crmCustomer.Nombre;
            customer.PaymentTermCode = crmCustomer.PaymentTermCode;
            customer.PaymentTermDescription = crmCustomer.PaymentTermDescription;
            customer.PostalCode = crmCustomer.PostalCode;
            customer.Province = crmCustomer.Province;
            customer.Telephone = crmCustomer.Telephone;
            customer.OwnerId = crmCustomer.OwnerId;
            customer.Active = true;
        }
    }
}
