﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Extensions;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class ServicesService : IServicesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IServiceData serviceData;
        private readonly ISessionManager sessionManager;
        private readonly ISolfacDelegateData solfacDelegateData;
        private readonly IServiceUpdateJobService serviceUpdateJobService;
        private readonly ILogMailer<ServicesService> logger;

        public ServicesService(IUnitOfWork unitOfWork,
            IServiceData serviceData,
            ISessionManager sessionManager,
            ISolfacDelegateData solfacDelegateData,
            ILogMailer<ServicesService> logger,
            IServiceUpdateJobService serviceUpdateJobService)
        {
            this.unitOfWork = unitOfWork;
            this.serviceData = serviceData;
            this.sessionManager = sessionManager;
            this.solfacDelegateData = solfacDelegateData;
            this.serviceUpdateJobService = serviceUpdateJobService;
            this.logger = logger;
        }

        public Response<List<Domain.Models.Billing.Service>> GetServices(string customerId)
        {
            var result = new List<Domain.Models.Billing.Service>();

            if (string.IsNullOrWhiteSpace(customerId)) return new Response<List<Domain.Models.Billing.Service>> { Data = result };

            var userNames = solfacDelegateData.GetUserDelegateByUserName(sessionManager.GetUserName());

            foreach (var item in userNames)
            {
                result.AddRange(serviceData.GetServices(customerId, item));
            }

            return new Response<List<Domain.Models.Billing.Service>> { Data = result };
        }

        public Response<List<SelectListModel>> GetServicesOptions(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId) || customerId == "null") return new Response<List<SelectListModel>> { Data = new List<SelectListModel>() };

            var result = GetServices(customerId);

            var response = new Response<List<SelectListModel>>
            {
                Data = result.Data
                    .Select(x => new SelectListModel { Id = x.CrmId, Text = x.Name })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return response;
        }

        public Response<ServiceModel> GetService(string serviceId, string customerId)
        {
            var result = GetServices(customerId).Data;

            var projects = unitOfWork.ProjectRepository.GetAllActives(serviceId);

            var opportunities = projects.Select(x => x.OpportunityNumber);

            var service = result.FirstOrDefault(x => x.CrmId.Equals(serviceId));

            var serviceModel = new ServiceModel(service) { Proposals = string.Join("-", opportunities) };

            return new Response<ServiceModel> { Data = serviceModel };
        }

        public Analytic GetAnalyticByService(string serviceId)
        {
            return unitOfWork.AnalyticRepository.GetByService(serviceId);
        }

        public Response<List<SelectListModel>> GetAllNotRelatedOptions(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId) || customerId == "null") return new Response<List<SelectListModel>> { Data = new List<SelectListModel>() };

            var result = unitOfWork.ServiceRepository.GetAllNotRelatedOptions(customerId);

            var response = new Response<List<SelectListModel>>
            {
                Data = result
                    .Select(x => new SelectListModel { Id = x.CrmId, Text = x.Name })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return response;
        }

        public Response Update()
        {
            var response = new Response();

            try
            {
                serviceUpdateJobService.Execute();
                response.AddSuccess(Resources.Common.UpdateSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }
    }
}
