using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Model.Models.Billing;
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

        public SolfacDelegateService(IUnitOfWork unitOfWork, ISessionManager sessionManager, IServiceData serviceData, IMapper mapper, IUserData userData, ICustomerData customerData)
        {
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.serviceData = serviceData;
            this.mapper = mapper;
            this.userData = userData;
            this.customerData = customerData;
        }

        private IEnumerable<SolfacDelegate> GetSolfacDelegatesByUser()
        {
            var userMail = sessionManager.GetUserEmail();

            var customers = customerData.GetCustomers(userMail);

            var serviceIds = new List<string>();
            foreach (var crmCustomer in customers)
            {
                var crmServices = serviceData.GetServices(crmCustomer.Id, userMail);

                serviceIds.AddRange(crmServices.Select(crmService => crmService.Id));
            }

            return unitOfWork.SolfacDelegateRepository.GetByServiceIds(serviceIds);
        }

        public Response<List<SolfacDelegateModel>> GetAll()
        {
            var data = GetSolfacDelegatesByUser();

            var items = new List<SolfacDelegateModel>();
            foreach (var solfacDelegate in data)
            {
                var model = Translate(solfacDelegate);
                var service = serviceData.GetService(solfacDelegate.ServiceId);
                var user = userData.GetById(solfacDelegate.UserId);

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

        public Response<SolfacDelegate> Save(SolfacDelegate solfacDelegate)
        {
            var response = ValidateSave();

            if (response.HasErrors()) return response;

            solfacDelegate.CreatedUser = sessionManager.GetUserName();

            response.Data = unitOfWork.SolfacDelegateRepository.Save(solfacDelegate);

            return response;
        }

        public Response Delete(int solfacDeletegateId)
        {
            unitOfWork.SolfacDelegateRepository.Delete(solfacDeletegateId);

            return new Response();
        }

        private Response<SolfacDelegate> ValidateSave()
        {
            var respone = new Response<SolfacDelegate>();

            var userName = sessionManager.GetUserName();

            var isValid = unitOfWork.UserRepository.HasManagerGroup(userName);

            if (!isValid)
            {
                respone.AddError(Resources.Billing.Solfac.SolfacDelegateMangerOnlyError);
            }

            return respone;
        }

        private SolfacDelegateModel Translate(SolfacDelegate solfacDelegate)
        {
            return mapper.Map<SolfacDelegate, SolfacDelegateModel>(solfacDelegate);
        }
    }
}
