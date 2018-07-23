using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Extensions;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Models;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Models.AllocationManagement;
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

        public Response<List<CrmService>> GetServices(string customerId, bool getAll)
        {
            var result = new List<CrmService>();

            if (string.IsNullOrWhiteSpace(customerId)) return new Response<List<CrmService>> { Data = result };

            var userNames = solfacDelegateData.GetUserDelegateByUserName(sessionManager.GetUserName());
            
            foreach (var item in userNames)
            {
                result.AddRange(serviceData.GetServices(customerId, item, getAll));
            }
            return new Response<List<CrmService>> { Data = result.DistinctBy(x => x.Id) };
        }

        public Response<List<SelectListModel>> GetServicesOptions(string customerId, bool getAll)
        {
            if (string.IsNullOrWhiteSpace(customerId) || customerId == "null") return new Response<List<SelectListModel>> { Data = new List<SelectListModel>() };

            var result = GetServices(customerId, getAll);

            var response = new Response<List<SelectListModel>>
            {
                Data = result.Data
                    .Select(x => new SelectListModel { Id = x.Id, Text = x.Nombre })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return response;
        }

        public Response<CrmService> GetService(string serviceId, string customerId)
        {
            var result = GetServices(customerId, false).Data;

            return new Response<CrmService> { Data = result.FirstOrDefault(x => x.Id.Equals(serviceId)) };
        }

        public Analytic GetAnalyticByService(string serviceId)
        {
            return unitOfWork.AnalyticRepository.GetByService(serviceId);
        }
    }
}
