using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Rrhh
{
    public class LicenseViewDelegateService : ILicenseViewDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IServiceData serviceData;
        private readonly ICustomerData customerData;
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;
        private readonly ILogMailer<LicenseViewDelegateService> logger;

        public LicenseViewDelegateService(IUnitOfWork unitOfWork, IUserData userData, IServiceData serviceData, ICustomerData customerData, ISessionManager sessionManager, IMapper mapper, ILogMailer<LicenseViewDelegateService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.serviceData = serviceData;
            this.customerData = customerData;
            this.sessionManager = sessionManager;
            this.mapper = mapper;
            this.logger = logger;
        }

        public Response<List<LicenseViewDelegateModel>> GetAll()
        {
            var response = new Response<List<LicenseViewDelegateModel>>();

            try
            {
                var data = GetUserDelegatesByUser();

                response.Data = data.Select(Translate).ToList();
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

                userDelegate.Type = UserDelegateType.LicenseView;

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

        private IEnumerable<UserDelegate> GetUserDelegatesByUser()
        {
            var userMail = sessionManager.GetUserEmail();

            var customers = customerData.GetCustomers(userMail);

            var serviceIds = new List<string>();
            foreach (var crmCustomer in customers)
            {
                var crmServices = serviceData.GetServices(crmCustomer.CrmId, userMail);

                serviceIds.AddRange(crmServices.Select(crmService => crmService.CrmId));
            }

            return unitOfWork.UserDelegateRepository.GetByServiceIds(serviceIds, UserDelegateType.LicenseView);
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

        private LicenseViewDelegateModel Translate(UserDelegate userDelegate)
        {
            var model = mapper.Map<UserDelegate, LicenseViewDelegateModel>(userDelegate);
            var service = serviceData.GetService(userDelegate.ServiceId);
            var user = userData.GetById(userDelegate.UserId);

            model.ManagerName = service.Manager;
            model.ServiceName = service.Name;
            model.UserName = user.Name;
            return model;
        }
    }
}
