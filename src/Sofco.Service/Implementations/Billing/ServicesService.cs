using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Models;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class ServicesService : IServicesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IServiceData serviceData;
        private readonly ISessionManager sessionManager;
        private readonly ISolfacDelegateData solfacDelegateData;

        public ServicesService(IUnitOfWork unitOfWork, IServiceData serviceData, ISessionManager sessionManager, ISolfacDelegateData solfacDelegateData)
        {
            this.unitOfWork = unitOfWork;
            this.serviceData = serviceData;
            this.sessionManager = sessionManager;
            this.solfacDelegateData = solfacDelegateData;
        }

        public Response<List<CrmService>> GetServices(string customerId)
        {
            var userNames = solfacDelegateData.GetUserDelegateByUserName(sessionManager.GetUserName());
            var result = new List<CrmService>();
            foreach (var item in userNames)
            {
                result.AddRange(serviceData.GetServices(customerId, item));
            }
            return new Response<List<CrmService>> {Data = result};
        }

        public Response<List<SelectListModel>> GetServicesOptions(string customerId)
        {
            var result = GetServices(customerId);

            var response = new Response<List<SelectListModel>>
            {
                Data = result.Data
                    .Select(x => new SelectListModel { Value = x.Id, Text = x.Nombre })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return response;
        }

        public Response<CrmService> GetService(string serviceId, string customerId)
        {
            var result = GetServices(customerId).Data;

            return new Response<CrmService> { Data = result.FirstOrDefault(x => x.Id.Equals(serviceId)) };
        }

        public bool HasAnalyticRelated(string serviceId)
        {
            return unitOfWork.AnalyticRepository.ExistWithService(serviceId);
        }
    }
}
