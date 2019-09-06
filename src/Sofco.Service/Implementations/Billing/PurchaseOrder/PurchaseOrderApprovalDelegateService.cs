using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Billing.PurchaseOrder;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Billing.PurchaseOrder
{
    public class PurchaseOrderApprovalDelegateService : IPurchaseOrderApprovalDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IAreaData areaData;
        private readonly ISectorData sectorData;
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;
        private readonly ILogMailer<PurchaseOrderApprovalDelegateService> logger;
        private List<UserDelegateType> types;
        private Dictionary<UserDelegateType, Action<PurchaseOrderApprovalDelegateModel>> resolverSourceDicts;
        private readonly EmailConfig emailConfig;
        private readonly AppSetting appSetting;

        public PurchaseOrderApprovalDelegateService(IUnitOfWork unitOfWork, IUserData userData, IAreaData areaData, IMapper mapper, ISectorData sectorData, ILogMailer<PurchaseOrderApprovalDelegateService> logger, ISessionManager sessionManager, IOptions<EmailConfig> emailOptions, IOptions<AppSetting> appSettingOption)
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
            emailConfig = emailOptions.Value;
            appSetting = appSettingOption.Value;
        }

        public Response<List<PurchaseOrderApprovalDelegateModel>> GetAll()
        {
            var response = new Response<List<PurchaseOrderApprovalDelegateModel>>();
            try
            {
                var currentUser = userData.GetCurrentUser();

                var data = unitOfWork.UserDelegateRepository.GetByTypesAndSourceId(types, currentUser.Id);

                response.Data = ResolveData(Translate(data));
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.GeneralError);
                logger.LogError(e);
            }

            return response;
        }

        public Response<PurchaseOrderApprovalDelegateModel> Save(PurchaseOrderApprovalDelegateModel userApprovalDelegate)
        {
            var response = ValidateSave(userApprovalDelegate);

            if (response.HasErrors()) return response;

            try
            {
                userApprovalDelegate.CreatedUser = sessionManager.GetUserName();

                unitOfWork.UserDelegateRepository.Save(Translate(userApprovalDelegate));

                response.Data = userApprovalDelegate;
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

        public Response<List<UserSelectListItem>> GetComplianceUsers()
        {
            var result = unitOfWork.UserRepository.GetByGroup(emailConfig.ComplianceCode)
                .Where(s => s.Email != sessionManager.GetUserEmail())
                .ToList();

            return new Response<List<UserSelectListItem>>
            {
                Data = Translate(result)
            };
        }

        public Response<List<UserSelectListItem>> GetDafUsers()
        {
            var result = unitOfWork.UserRepository.GetByGroup(emailConfig.DafCode)
                .Where(s => s.Email != sessionManager.GetUserEmail())
                .ToList();

            return new Response<List<UserSelectListItem>>
            {
                Data = Translate(result)
            };
        }

        private void SetTypes()
        {
            types = new List<UserDelegateType>
            {
                UserDelegateType.PurchaseOrderApprovalCommercial,
                UserDelegateType.PurchaseOrderApprovalCompliance,
                UserDelegateType.PurchaseOrderApprovalDaf,
                UserDelegateType.PurchaseOrderApprovalOperation
            };
        }

        private void SetResolverSourceDicts()
        {
            resolverSourceDicts = new Dictionary<UserDelegateType, Action<PurchaseOrderApprovalDelegateModel>>
            {
                {UserDelegateType.PurchaseOrderApprovalCommercial, ResolveSourceCommercial},
                {UserDelegateType.PurchaseOrderApprovalOperation, ResolveSourceOperation},
                {UserDelegateType.PurchaseOrderApprovalDaf, ResolveSourceUser},
                {UserDelegateType.PurchaseOrderApprovalCompliance, ResolveSourceUser}
            };
        }

        private List<PurchaseOrderApprovalDelegateModel> Translate(List<UserDelegate> data)
        {
            return mapper.Map<List<UserDelegate>, List<PurchaseOrderApprovalDelegateModel>>(data);
        }

        private UserDelegate Translate(PurchaseOrderApprovalDelegateModel model)
        {
            return mapper.Map<PurchaseOrderApprovalDelegateModel, UserDelegate>(model);
        }

        private List<PurchaseOrderApprovalDelegateModel> ResolveData(List<PurchaseOrderApprovalDelegateModel> userDelegates)
        {
            ResolveSource(userDelegates);

            ResolveUsers(userDelegates);

            return userDelegates;
        }

        private void ResolveSource(List<PurchaseOrderApprovalDelegateModel> userDelegates)
        {
            foreach (var userDelegate in userDelegates)
            {
                if (userDelegate.SourceId == 0) continue;

                if(!resolverSourceDicts.ContainsKey(userDelegate.Type)) continue;

                resolverSourceDicts[userDelegate.Type](userDelegate);
            }
        }

        private void ResolveSourceUser(PurchaseOrderApprovalDelegateModel purchaseOrderApprovalDelegate)
        {
            var data = userData.GetUserLiteById(purchaseOrderApprovalDelegate.SourceId);

            if(data == null) return;

            purchaseOrderApprovalDelegate.SourceName = data.Name;

            purchaseOrderApprovalDelegate.ResponsableId = data.Id;
        }

        private void ResolveSourceCommercial(PurchaseOrderApprovalDelegateModel purchaseOrderApprovalDelegate)
        {
            var data = areaData.GetAll().FirstOrDefault(s => s.ResponsableUserId == purchaseOrderApprovalDelegate.SourceId);

            if(data == null) return;

            purchaseOrderApprovalDelegate.SourceName = data.Text;

            purchaseOrderApprovalDelegate.ResponsableId = data.ResponsableUserId;
        }

        private void ResolveSourceOperation(PurchaseOrderApprovalDelegateModel purchaseOrderApprovalDelegate)
        {
            var data = sectorData.GetAll().FirstOrDefault(s => s.ResponsableUserId == purchaseOrderApprovalDelegate.SourceId);

            if(data == null) return;

            purchaseOrderApprovalDelegate.SourceName = data.Text;

            purchaseOrderApprovalDelegate.ResponsableId = data.ResponsableUserId;
        }

        private void ResolveUsers(List<PurchaseOrderApprovalDelegateModel> userDelegates)
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

        private Response<PurchaseOrderApprovalDelegateModel> ValidateSave(PurchaseOrderApprovalDelegateModel model)
        {
            var respone = new Response<PurchaseOrderApprovalDelegateModel>();

            var validResponse = ValidateData(model);
            if (validResponse.HasErrors())
            {
                respone.AddMessages(validResponse.Messages);

                return respone;
            }

            validResponse = ValidateSameUser(model);
            if (validResponse.HasErrors())
            {
                respone.AddMessages(validResponse.Messages);
            }

            return respone;
        }

        private Response ValidateData(PurchaseOrderApprovalDelegateModel model)
        {
            var response = new Response();

            var isValid = !(model.ResponsableId == 0 || model.UserId == 0 || model.SourceId == 0);

            if (!isValid)
            {
                response.AddError(Resources.Billing.PurchaseOrder.DelegateWrongDataError);
            }

            return response;
        }

        private Response ValidateSameUser(PurchaseOrderApprovalDelegateModel model)
        {
            var response = new Response();

            var isValid = true;

            if (model.Type == UserDelegateType.PurchaseOrderApprovalDaf || model.Type == UserDelegateType.PurchaseOrderApprovalCompliance)
            {
                if (model.ResponsableId == model.UserId)
                {
                    isValid = false;
                }
            }

            if (model.Type == UserDelegateType.PurchaseOrderApprovalCommercial)
            {
                var item = areaData.GetAll().FirstOrDefault(s => s.Id == model.SourceId);

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

        private List<UserSelectListItem> Translate(List<User> users)
        {
            return mapper.Map<List<User>, List<UserSelectListItem>>(users);
        }
    }
}
