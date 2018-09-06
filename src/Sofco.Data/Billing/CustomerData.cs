using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Cache;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Domain.Models.Billing;

namespace Sofco.Data.Billing
{
    public class CustomerData : ICustomerData
    {
        private const string CustomersCacheKey = "urn:customers:{0}:all";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly ISessionManager sessionManager;
        private readonly IUnitOfWork unitOfWork;

        public CustomerData(ICacheManager cacheManager, ISessionManager sessionManager, IUnitOfWork unitOfWork)
        {
            this.cacheManager = cacheManager;
            this.sessionManager = sessionManager;
            this.unitOfWork = unitOfWork;
        }

        public IList<Customer> GetCustomers(string username)
        {
            var email = sessionManager.GetUserEmail(username);

            return cacheManager.GetHashList(string.Format(CustomersCacheKey, username),
                () =>
                {
                    var hasDirectorGroup = unitOfWork.UserRepository.HasDirectorGroup(email);
                    var hasCommercialGroup = unitOfWork.UserRepository.HasComercialGroup(email);
                    var hasCdgGroup = unitOfWork.UserRepository.HasCdgGroup(email);
                    var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(email);

                    var hasAllAccess = hasDirectorGroup || hasCommercialGroup || hasCdgGroup || hasDafGroup;

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
