using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm;
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

        private IList<int> IdsAdded { get; }

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
            try
            {
                unitOfWork.CustomerRepository.Update(Translate(crmCustomer, customer));
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
            try
            {
                unitOfWork.CustomerRepository.Insert(Translate(crmCustomer));
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error on insert Customer: {crmCustomer.Name}", e);
            }
        }

        private Customer Translate(CrmCustomer data, Customer customer = null)
        {
            customer = customer ?? new Customer();

            var result = mapper.Map(data, customer);

            result.Active = true;

            return result;
        }

        private List<CrmCustomer> Translate(List<CrmAccount> data)
        {
            return mapper.Map<List<CrmAccount>, List<CrmCustomer>>(data);
        }
    }
}
