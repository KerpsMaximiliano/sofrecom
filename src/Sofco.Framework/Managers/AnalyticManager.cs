using System;
using Sofco.Core.Data.Admin;
using Sofco.Core.Managers;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Framework.Managers
{
    public class AnalyticManager : IAnalyticManager
    {
        private readonly IUserData userData;

        private readonly ICrmServiceService crmServiceService;

        public AnalyticManager(IUserData userData, ICrmServiceService crmServiceService)
        {
            this.userData = userData;
            this.crmServiceService = crmServiceService;
        }

        public Response UpdateCrmAnalytic(Analytic analytic)
        {
            var response = new Response();
             
            var analyticTitle = analytic.Title;

            var serviceId = analytic.ServiceId;

            var managerId = GetExternalManagerId(analytic);

            if (analytic.ManagerId.HasValue && !managerId.HasValue)
            {
                response.AddError(Resources.AllocationManagement.Analytic.NoCrmUpdateNullManagerId);

                return response;
            }

            var dataUpdate = new CrmServiceUpdate
            {
                Id = new Guid(serviceId),
                ManagerId = managerId,
                AnalyticTitle = analyticTitle,
                ServiceTypeId = analytic.ServiceTypeId,
                SoluctionTypeId = analytic.SolutionId,
                TechnologyTypeId = analytic.TechnologyId
            };

            var result = crmServiceService.Update(dataUpdate);
            if (result.HasErrors)
            {
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private Guid? GetExternalManagerId(Analytic analytic)
        {
            if (!analytic.ManagerId.HasValue) return null;

            var managerId = analytic.ManagerId.Value;

            var user = userData.GetById(managerId);

            if (user == null || string.IsNullOrEmpty(user.ExternalManagerId)) return null;

            return Guid.Parse(user.ExternalManagerId);
        }
    }
}
