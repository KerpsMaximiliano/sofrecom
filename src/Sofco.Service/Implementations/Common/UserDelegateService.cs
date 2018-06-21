using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Models.Common;
using Sofco.Core.Services.Common;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Common
{
    public class UserDelegateService : IUserDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IServiceData serviceData;
        private readonly ICustomerData customerData;
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;

        public UserDelegateService(IUnitOfWork unitOfWork, ISessionManager sessionManager, IServiceData serviceData, IMapper mapper, IUserData userData, ICustomerData customerData)
        {
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.serviceData = serviceData;
            this.mapper = mapper;
            this.userData = userData;
            this.customerData = customerData;
        }

        public Response<List<UserDelegateModel>> GetAll(UserDelegateType type)
        {
            var data = GetUserDelegatesByUser(type);

            var items = new List<UserDelegateModel>();
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

            return new Response<List<UserDelegateModel>> { Data = items };
        }

        public Response<UserDelegate> Save(UserDelegate userDelegate)
        {
            var response = ValidateSave();

            if (response.HasErrors()) return response;

            userDelegate.CreatedUser = sessionManager.GetUserName();

            response.Data = unitOfWork.UserDelegateRepository.Save(userDelegate);

            return response;
        }

        public Response Delete(int userDeletegateId)
        {
            unitOfWork.UserDelegateRepository.Delete(userDeletegateId);

            return new Response();
        }

        private IEnumerable<UserDelegate> GetUserDelegatesByUser(UserDelegateType type)
        {
            var userMail = sessionManager.GetUserEmail();

            var customers = customerData.GetCustomers(userMail, false);

            var serviceIds = new List<string>();
            foreach (var crmCustomer in customers)
            {
                var crmServices = serviceData.GetServices(crmCustomer.Id, userMail);

                serviceIds.AddRange(crmServices.Select(crmService => crmService.Id));
            }

            return unitOfWork.UserDelegateRepository.GetByServiceIds(serviceIds, type);
        }

        private Response<UserDelegate> ValidateSave()
        {
            var respone = new Response<UserDelegate>();

            var userName = sessionManager.GetUserName();

            var isValid = unitOfWork.UserRepository.HasManagerGroup(userName);

            if (!isValid)
            {
                respone.AddError(Resources.AllocationManagement.TimeApproval.UserDelegateMangerOnlyError);
            }

            return respone;
        }

        private UserDelegateModel Translate(UserDelegate userDelegate)
        {
            return mapper.Map<UserDelegate, UserDelegateModel>(userDelegate);
        }
    }
}
