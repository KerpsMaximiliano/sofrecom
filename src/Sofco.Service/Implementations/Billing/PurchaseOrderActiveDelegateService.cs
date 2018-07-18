using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class PurchaseOrderActiveDelegateService : IPurchaseOrderActiveDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IAnalyticData analyticData;
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;
        private readonly ILogMailer<PurchaseOrderActiveDelegateService> logger;

        public PurchaseOrderActiveDelegateService(IUnitOfWork unitOfWork, 
            ISessionManager sessionManager, 
            ILogMailer<PurchaseOrderActiveDelegateService> logger,
            IMapper mapper,
            IUserData userData, 
            IAnalyticData analyticData)
        {
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.mapper = mapper;
            this.userData = userData;
            this.analyticData = analyticData;
            this.logger = logger;
        }

        public Response<List<PurchaseOrderActiveDelegateModel>> GetAll()
        {
            var response = new Response<List<PurchaseOrderActiveDelegateModel>>();

            try
            {
                var currentUser = userData.GetCurrentUser();

                var analytics = analyticData.GetByManagerId(currentUser.Id);

                var serviceIds = analytics.Select(item => item.ServiceId).ToList();

                var data = unitOfWork.UserDelegateRepository.GetByServiceIds(serviceIds, UserDelegateType.PurchaseOrderActive);

                var items = data.Select(userDelegate => Translate(userDelegate, analytics)).ToList();

                response.Data = items;
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<UserDelegate> Save(UserDelegate userDelegate)
        {
            var response = ValidateSave();

            if (response.HasErrors()) return response;

            try
            {
                userDelegate.CreatedUser = sessionManager.GetUserName();

                userDelegate.Type = UserDelegateType.PurchaseOrderActive;

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

        private PurchaseOrderActiveDelegateModel Translate(UserDelegate userDelegate, List<Analytic> analytics)
        {
            var model = mapper.Map<UserDelegate, PurchaseOrderActiveDelegateModel>(userDelegate);
            var analytic = analytics.FirstOrDefault(s => s.ServiceId == userDelegate.ServiceId.ToString());
            if (analytic == null) return model;
            model.ServiceName = analytic.Service;
            if (analytic.ManagerId.HasValue)
            {
                var managerUser = userData.GetUserLiteById(analytic.ManagerId.Value);
                model.ManagerName = managerUser.Name;
            }
            var user = userData.GetUserLiteById(userDelegate.UserId);
            model.UserName = user.Name;
            return model;
        }
    }
}
