using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
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
        private readonly EmailConfig emailConfig;

        public ServicesService(IUnitOfWork unitOfWork, IServiceData serviceData, IOptions<EmailConfig> emailConfig)
        {
            this.unitOfWork = unitOfWork;
            this.serviceData = serviceData;
            this.emailConfig = emailConfig.Value;
        }

        public IList<CrmService> GetServices(string customerId, string userMail, string userName)
        {
            var hasDirectorGroup = this.unitOfWork.UserRepository.HasDirectorGroup(userMail);
            var hasCommercialGroup = this.unitOfWork.UserRepository.HasComercialGroup(emailConfig.ComercialCode, userMail);

            return serviceData.GetServices(customerId, userName, userMail, hasDirectorGroup || hasCommercialGroup);
        }
    }
}
