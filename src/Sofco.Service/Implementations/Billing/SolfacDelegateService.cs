﻿using System;
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
    public class SolfacDelegateService : ISolfacDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IServiceData serviceData;
        private readonly ICustomerData customerData;
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;
        private readonly ILogMailer<SolfacDelegateService> logger;

        public SolfacDelegateService(IUnitOfWork unitOfWork, 
            ISessionManager sessionManager, 
            IServiceData serviceData,
            ILogMailer<SolfacDelegateService> logger,
            IMapper mapper, 
            IUserData userData, 
            ICustomerData customerData)
        {
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.serviceData = serviceData;
            this.mapper = mapper;
            this.userData = userData;
            this.customerData = customerData;
        }

        private IEnumerable<UserDelegate> GetUserDelegatesByUser()
        {
            var userMail = sessionManager.GetUserEmail();

            var customers = customerData.GetCustomers(userMail);

            var serviceIds = new List<string>();
            foreach (var crmCustomer in customers)
            {
                var crmServices = serviceData.GetServices(crmCustomer.Id, userMail);

                serviceIds.AddRange(crmServices.Select(crmService => crmService.Id));
            }

            return unitOfWork.UserDelegateRepository.GetByServiceIds(serviceIds, UserDelegateType.Solfac);
        }

        public Response<List<SolfacDelegateModel>> GetAll()
        {
            var data = GetUserDelegatesByUser();

            var items = new List<SolfacDelegateModel>();
            foreach (var userDelegate in data)
            {
                var model = Translate(userDelegate);
                var service = serviceData.GetService(userDelegate.ServiceId);
                var user = userData.GetById(userDelegate.UserId);

                model.ManagerName = service.Manager;
                model.ServiceName = service.Nombre;
                model.UserName = user.Name;

                items.Add(model);
            }

            var response = new Response<List<SolfacDelegateModel>>
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

        private SolfacDelegateModel Translate(UserDelegate solfacDelegate)
        {
            return mapper.Map<UserDelegate, SolfacDelegateModel>(solfacDelegate);
        }
    }
}
