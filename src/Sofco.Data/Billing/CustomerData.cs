using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Cache;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Domain.Models.Billing;
using Sofco.Core.Managers;

namespace Sofco.Data.Billing
{
    public class CustomerData : ICustomerData
    {
        private const string CustomersCacheKey = "urn:customers:{0}:all";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly ISessionManager sessionManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRoleManager roleManager;

        public CustomerData(ICacheManager cacheManager, ISessionManager sessionManager, IUnitOfWork unitOfWork, IRoleManager roleManager)
        {
            this.cacheManager = cacheManager;
            this.sessionManager = sessionManager;
            this.unitOfWork = unitOfWork;
            this.roleManager = roleManager;
        }

        public IList<Customer> GetCustomers(string username)
        {
            var email = sessionManager.GetUserEmail(username);

            return cacheManager.GetHashList(string.Format(CustomersCacheKey, username),
                () =>
                {
                    var hasAllAccess = roleManager.HasFullAccess();

                    if (hasAllAccess)
                        return unitOfWork.CustomerRepository.GetAllActives();
                    else
                    {
                        var user = unitOfWork.UserRepository.GetByEmail(email);
                        var services = unitOfWork.ServiceRepository.GetAllByManager(user.ExternalManagerId);

                        var servicesIds = services.Select(x => x.AccountId);

                        return unitOfWork.CustomerRepository.GetAllByServices(servicesIds);
                    }
                },
                x => x.CrmId,
                cacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(CustomersCacheKey, '*'));
        }
    }
}
