using System;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.Billing;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class ContactUpdateJobService : IContactUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ContactUpdateJobService> logger;
        private readonly ICrmContactService crmContactService;
        private readonly IMapper mapper;

        public ContactUpdateJobService(IUnitOfWork unitOfWork,
            ILogMailer<ContactUpdateJobService> logger,
            ICrmContactService crmContactService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.crmContactService = crmContactService;
            this.mapper = mapper;
        }

        public void Execute()
        {
            var crmContacts = crmContactService.GetAll();

            foreach (var crmContact in crmContacts)
            {
                var contact = unitOfWork.ContactRepository.GetByCrmId(crmContact.Id);

                if (contact == null)
                    Create(crmContact);
                else
                    Update(crmContact, contact);
            }

            try
            {
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void Update(CrmContact crmContact, Contact contact)
        {
            try
            {
                unitOfWork.ContactRepository.Update(Translate(crmContact, contact));
            }
            catch (Exception e)
            {
                logger.LogError($"Error on update Contact: {crmContact.Name}", e);
            }
        }

        private void Create(CrmContact crmContact)
        {
            try
            {
                unitOfWork.ContactRepository.Insert(Translate(crmContact));
            }
            catch (Exception e)
            {
                logger.LogError($"Error on insert Contact: {crmContact.Name}", e);
            }
        }

        private Contact Translate(CrmContact data, Contact contact = null)
        {
            contact = contact ?? new Contact();

            var result = mapper.Map(data, contact);

            return result;
        }
    }
}
