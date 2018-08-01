using System;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Cache;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Data.AllocationManagement
{
    public class EmployeeData : IEmployeeData
    {
        private const string EmployeeByEmailCacheKey = "urn:employees:email:{0}";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly ISessionManager sessionManager;

        private readonly IUnitOfWork unitOfWork;

        public EmployeeData(ICacheManager cacheManager, IUnitOfWork unitOfWork, ISessionManager sessionManager)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
        }

        public Employee GetCurrentEmployee()
        {
            var email = sessionManager.GetUserEmail();

            return cacheManager.Get(string.Format(EmployeeByEmailCacheKey, email),
                () => unitOfWork.EmployeeRepository.GetSingle(x => x.Email == email),
                cacheExpire);
        }
    }
}
