using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class PurchaseOrderDelegateService : IPurchaseOrderDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IAreaData areaData;
        private readonly ISectorData sectorData;
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;
        private readonly ILogMailer<PurchaseOrderDelegateService> logger;
        private List<UserDelegateType> types;
        private Dictionary<UserDelegateType, Action<PurchaseOrderDelegateModel>> resolverSourceDicts;

        public PurchaseOrderDelegateService(IUnitOfWork unitOfWork, IUserData userData, IAreaData areaData, IMapper mapper, ISectorData sectorData, ILogMailer<PurchaseOrderDelegateService> logger, ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userData = userData;
            this.areaData = areaData;
            this.sectorData = sectorData;
            this.logger = logger;
            this.sessionManager = sessionManager;
            SetTypes();
            SetResolverSourceDicts();
        }

        public Response<List<PurchaseOrderDelegateModel>> GetAll()
        {
            var response = new Response<List<PurchaseOrderDelegateModel>>();
            try
            {
                var data = unitOfWork.UserDelegateRepository.GetByTypes(types);

                response.Data = ResolveData(Translate(data));
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.GeneralError);
                logger.LogError(e);
            }

            return response;
        }

        public Response<PurchaseOrderDelegateModel> Save(PurchaseOrderDelegateModel userDelegate)
        {
            var response = ValidateSave(userDelegate);

            if (response.HasErrors()) return response;

            try
            {
                userDelegate.CreatedUser = sessionManager.GetUserName();

                unitOfWork.UserDelegateRepository.Save(Translate(userDelegate));

                response.Data = userDelegate;
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

        private void SetTypes()
        {
            types = new List<UserDelegateType>
            {
                UserDelegateType.PurchaseOrderCommercial,
                UserDelegateType.PurchaseOrderCompliance,
                UserDelegateType.PurchaseOrderDaf,
                UserDelegateType.PurchaseOrderOperation
            };
        }

        private void SetResolverSourceDicts()
        {
            resolverSourceDicts = new Dictionary<UserDelegateType, Action<PurchaseOrderDelegateModel>>
            {
                {UserDelegateType.PurchaseOrderCommercial, ResolveSourceCommercial},
                {UserDelegateType.PurchaseOrderOperation, ResolveSourceOperation}
            };
        }

        private List<PurchaseOrderDelegateModel> Translate(List<UserDelegate> data)
        {
            return mapper.Map<List<UserDelegate>, List<PurchaseOrderDelegateModel>>(data);
        }

        private UserDelegate Translate(PurchaseOrderDelegateModel model)
        {
            return mapper.Map<PurchaseOrderDelegateModel, UserDelegate>(model);
        }

        private List<PurchaseOrderDelegateModel> ResolveData(List<PurchaseOrderDelegateModel> userDelegates)
        {
            ResolveSource(userDelegates);

            ResolveUsers(userDelegates);

            return userDelegates;
        }

        private void ResolveSource(List<PurchaseOrderDelegateModel> userDelegates)
        {
            foreach (var userDelegate in userDelegates)
            {
                if (userDelegate.SourceId == 0) continue;

                if(!resolverSourceDicts.ContainsKey(userDelegate.Type)) continue;

                resolverSourceDicts[userDelegate.Type](userDelegate);
            }
        }

        private void ResolveSourceCommercial(PurchaseOrderDelegateModel purchaseOrderDelegate)
        {
            var area = areaData.GetAll().FirstOrDefault(s => s.Id == purchaseOrderDelegate.SourceId);

            if(area == null) return;

            purchaseOrderDelegate.SourceName = area.Text;

            purchaseOrderDelegate.ResponsableId = area.ResponsableUserId;
        }

        private void ResolveSourceOperation(PurchaseOrderDelegateModel purchaseOrderDelegate)
        {
            var area = sectorData.GetAll().FirstOrDefault(s => s.Id == purchaseOrderDelegate.SourceId);

            if(area == null) return;

            purchaseOrderDelegate.SourceName = area.Text;

            purchaseOrderDelegate.ResponsableId = area.ResponsableUserId;
        }

        private void ResolveUsers(List<PurchaseOrderDelegateModel> userDelegates)
        {
            foreach (var userDelegate in userDelegates)
            {
                var user = userData.GetUserLiteById(userDelegate.UserId);

                userDelegate.UserName = user?.Name;

                if(userDelegate.ResponsableId == 0) continue;

                var responsable = userData.GetUserLiteById(userDelegate.ResponsableId);

                userDelegate.ResponsableName = responsable?.Name;
            }
        }

        private Response<PurchaseOrderDelegateModel> ValidateSave(PurchaseOrderDelegateModel model)
        {
            var respone = new Response<PurchaseOrderDelegateModel>();

            var validResponse = ValidateSameUser(model);
            if (validResponse.HasErrors())
            {
                respone.AddMessages(validResponse.Messages);
            }

            return respone;
        }

        private Response ValidateSameUser(PurchaseOrderDelegateModel model)
        {
            var response = new Response();

            var isValid = true;

            if (model.Type == UserDelegateType.PurchaseOrderCommercial)
            {
                var item = areaData.GetAll().FirstOrDefault(s => s.Id == model.SourceId);

                if (item == null) return response;

                if (item.ResponsableUserId == model.UserId)
                {
                    isValid = false;
                }
            }

            if (model.Type == UserDelegateType.PurchaseOrderOperation)
            {
                var item = sectorData.GetAll().FirstOrDefault(s => s.Id == model.SourceId);

                if (item == null) return response;

                if (item.ResponsableUserId == model.UserId)
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                response.AddError(Resources.Billing.PurchaseOrder.DelegateSameUserError);
            }

            return response;
        }
    }
}
