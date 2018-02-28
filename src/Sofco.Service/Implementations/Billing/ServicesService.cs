using System.Collections.Generic;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Service.Implementations.Billing
{
    public class ServicesService : IServicesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IServiceData serviceData;
        private readonly ISessionManager sessionManager;

        public ServicesService(IUnitOfWork unitOfWork, IServiceData serviceData, ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.serviceData = serviceData;
            this.sessionManager = sessionManager;
        }

        public IList<CrmService> GetServices(string customerId)
        {
            return serviceData.GetServices(customerId, sessionManager.GetUserMail());
        }

        public bool HasAnalyticRelated(string serviceId)
        {
            return unitOfWork.AnalyticRepository.ExistWithService(serviceId);
        }
    }
}
