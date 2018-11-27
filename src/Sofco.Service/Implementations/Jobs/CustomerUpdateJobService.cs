using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Domain.Models.Billing;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class CustomerUpdateJobService : ICustomerUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CustomerUpdateJobService> logger;
        private readonly ICustomerData customerData;
        private readonly ICrmAccountService crmAccountService;
        private readonly IMapper mapper;

        private IList<int> IdsAdded { get; set; }

        public CustomerUpdateJobService(IUnitOfWork unitOfWork, 
            ICustomerData customerData,
            ILogMailer<CustomerUpdateJobService> logger, 
            ICrmAccountService crmAccountService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.crmAccountService = crmAccountService;
            this.mapper = mapper;
            this.customerData = customerData;

            IdsAdded = new List<int>();
        }

        public void Execute()
        {
            var crmAccounts = Translate(crmAccountService.GetAll());

            unitOfWork.BeginTransaction();

            foreach (var crmCustomer in crmAccounts)
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
                logger.LogError($"Error on update Customer: {crmCustomer.Name}", e);
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
                logger.LogError($"Error on insert Customer: {crmCustomer.Name}", e);
            }
        }

        private void FillData(Customer customer, CrmCustomer crmCustomer)
        {
            customer.CrmId = crmCustomer.Id;
            customer.Address = crmCustomer.Address;
            customer.Cuit = crmCustomer.Cuit;
            customer.City = crmCustomer.City;
            customer.Contact = crmCustomer.Contact;
            customer.Country = crmCustomer.Country;
            customer.CurrencyDescription = crmCustomer.CurrencyDescription;
            customer.CurrencyId = crmCustomer.CurrencyId;
            customer.Name = crmCustomer.Name;
            customer.PaymentTermCode = crmCustomer.PaymentTermCode;
            customer.PaymentTermDescription = crmCustomer.PaymentTermDescription;
            customer.PostalCode = crmCustomer.PostalCode;
            customer.Province = crmCustomer.Province;
            customer.Telephone = crmCustomer.Telephone;
            customer.OwnerId = crmCustomer.OwnerId;
            customer.Active = true;
        }

        private List<CrmCustomer> Translate(List<CrmAccount> data)
        {
            return mapper.Map<List<CrmAccount>, List<CrmCustomer>>(data);
        }
    }
}
