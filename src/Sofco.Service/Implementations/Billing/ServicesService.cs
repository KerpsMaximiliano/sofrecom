using System.Collections.Generic;
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

        public ServicesService(IUnitOfWork unitOfWork, IServiceData serviceData)
        {
            this.unitOfWork = unitOfWork;
            this.serviceData = serviceData;
        }

        public IList<CrmService> GetServices(string customerId, string userMail, string userName)
        {
            var hasDirectorGroup = this.unitOfWork.UserRepository.HasDirectorGroup(userMail);

            return serviceData.GetServices(customerId, userName, userMail, hasDirectorGroup);
        }
    }
}
