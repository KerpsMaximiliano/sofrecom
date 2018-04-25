using System;
using Sofco.Core.Cache;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Data.AllocationManagement
{
    public class EmployeeData : IEmployeeData
    {
        private const string EmployeeByEmailCacheKey = "urn:employees:email:{0}";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly IUnitOfWork unitOfWork;

        public EmployeeData(ICacheManager cacheManager, IUnitOfWork unitOfWork)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
        }

        public Employee GetByEmail(string email)
        {
            return cacheManager.Get(string.Format(EmployeeByEmailCacheKey, email),
                () => unitOfWork.EmployeeRepository.GetSingle(x => x.Email == email),
                cacheExpire);
        }
    }
}
