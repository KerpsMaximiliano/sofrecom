using System.Collections.Generic;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Service.Implementations.Billing
{
    public class ServicesService : IServicesService
    {
        private readonly IUserRepository userRepository;
        private readonly IServiceData serviceData;

        public ServicesService(IUserRepository userRepository, IServiceData serviceData)
        {
            this.userRepository = userRepository;
            this.serviceData = serviceData;
        }

        public IList<CrmService> GetServices(string customerId, string userMail, string userName)
        {
            var hasDirectorGroup = this.userRepository.HasDirectorGroup(userMail);

            return serviceData.GetServices(customerId, userName, userMail, hasDirectorGroup);
        }
    }
}
