using System;
using System.Collections.Generic;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.DAL;
using Sofco.Core.Services.Billing;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class SolfacDelegateService : ISolfacDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISessionManager sessionManager;

        public SolfacDelegateService(IUnitOfWork unitOfWork, ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
        }

        public Response<List<SolfacDelegate>> GetByServiceId(Guid serviceId)
        {
            var response = new Response<List<SolfacDelegate>>
            {
                Data = unitOfWork.SolfacDelegateRepository.GetByServiceId(serviceId)
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
    }
}
