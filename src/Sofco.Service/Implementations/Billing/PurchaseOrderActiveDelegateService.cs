using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing;
using Sofco.Data.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class PurchaseOrderActiveDelegateService : IPurchaseOrderActiveDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IAnalyticData analyticData;
        private readonly IServiceData serviceData;
        private readonly ICustomerData customerData;
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;
        private readonly ILogMailer<PurchaseOrderActiveDelegateService> logger;

        public PurchaseOrderActiveDelegateService(IUnitOfWork unitOfWork,
            ISessionManager sessionManager,
            IServiceData serviceData,
            ILogMailer<PurchaseOrderActiveDelegateService> logger,
            IMapper mapper,
            IUserData userData,
            ICustomerData customerData, IAnalyticData analyticData)
        {
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.serviceData = serviceData;
            this.mapper = mapper;
            this.userData = userData;
            this.customerData = customerData;
            this.analyticData = analyticData;
            this.logger = logger;
        }

        public Response<List<PurchaseOrderActiveDelegateModel>> GetAll()
        {
            var currentUser = userData.GetCurrentUser();

            var analytics = analyticData.GetByManagerId(currentUser.Id);

            var serviceIds = analytics.Select(item => item.ServiceId).ToList();

            var data = unitOfWork.UserDelegateRepository.GetByServiceIds(serviceIds, UserDelegateType.PurchaseOrderActive);

            var items = new List<PurchaseOrderActiveDelegateModel>();
            foreach (var userDelegate in data)
            {
                var model = Translate(userDelegate);
                var analytic = analytics.FirstOrDefault(s => s.ServiceId == userDelegate.ServiceId.ToString());
                if(analytic == null) continue;
                model.ServiceName = analytic.Service;
                if (analytic.ManagerId.HasValue)
                {
                    var managerUser = userData.GetUserLiteById(analytic.ManagerId.Value);
                    model.ManagerName = managerUser.Name;
                }

                var user = userData.GetUserLiteById(userDelegate.UserId);
                model.UserName = user.Name;

                items.Add(model);
            }

            var response = new Response<List<PurchaseOrderActiveDelegateModel>>
            {
                Data = items
            };

            return response;
        }

        public Response<UserDelegate> Save(UserDelegate userDelegate)
        {
            var response = ValidateSave();

            if (response.HasErrors()) return response;

            try
            {
                userDelegate.CreatedUser = sessionManager.GetUserName();

                userDelegate.Type = UserDelegateType.Solfac;

                response.Data = unitOfWork.UserDelegateRepository.Save(userDelegate);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Delete(int userDeletegateId)
        {
            unitOfWork.UserDelegateRepository.Delete(userDeletegateId);

            return new Response();
        }

        private Response<UserDelegate> ValidateSave()
        {
            var respone = new Response<UserDelegate>();

            var userName = sessionManager.GetUserName();

            var isValid = unitOfWork.UserRepository.HasManagerGroup(userName);

            if (!isValid)
            {
                respone.AddError(Resources.Billing.Solfac.SolfacDelegateMangerOnlyError);
            }

            return respone;
        }

        private PurchaseOrderActiveDelegateModel Translate(UserDelegate solfacDelegate)
        {
            return mapper.Map<UserDelegate, PurchaseOrderActiveDelegateModel>(solfacDelegate);
        }
    }
}
