﻿using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(SofcoContext context) : base(context)
        {
        }

        public Service GetByIdCrm(string crmServiceId)
        {
            return context.Services.SingleOrDefault(x => x.CrmId.Equals(crmServiceId));
        }

        public IList<Service> GetAllActives(string customerId)
        {
            return context.Services.Where(x => x.AccountId.Equals(customerId) && x.Active).ToList().AsReadOnly();
        }

        public IList<Service> GetAllByManager(string customerId, string externalManagerId)
        {
            return context.Services.Where(x => x.Active && x.AccountId.Equals(customerId) && x.ManagerId.Equals(externalManagerId)).ToList().AsReadOnly();
        }

        public IList<Service> GetAllByManager(string externalManagerId)
        {
            return context.Services.Where(x => x.Active && x.ManagerId.Equals(externalManagerId)).ToList().AsReadOnly();
        }

        public void UpdateInactives(IList<int> idsAdded)
        {
            var services = context.Services.Where(x => !idsAdded.Contains(x.Id)).ToList();

            foreach (var service in services)
            {
                service.Active = false;
                context.Entry(service).Property("Active").IsModified = true;
            }

            context.SaveChanges();
        }

        public void UpdateActive(Service service)
        {
            context.Entry(service).Property("Active").IsModified = true;
        }

        public void UpdateAnalyticAndManager(Service service)
        {
            context.Entry(service).Property("Analytic").IsModified = true;
            context.Entry(service).Property("ManagerId").IsModified = true;
            context.Entry(service).Property("Manager").IsModified = true;
        }

        public IList<Service> GetAllNotRelatedOptions(string customerId)
        {
            return context.Services.Where(x => x.AccountId.Equals(customerId) && x.Active && string.IsNullOrWhiteSpace(x.Analytic)).ToList().AsReadOnly();
        }
    }
}
