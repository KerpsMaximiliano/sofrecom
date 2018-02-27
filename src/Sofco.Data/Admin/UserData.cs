using System;
using Sofco.Core.Cache;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Model.Models.Admin;

namespace Sofco.Data.Admin
{
    public class UserData : IUserData
    {
        private const string UserByIdCacheKey = "urn:users:{0}";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly IUnitOfWork unitOfWork;

        public UserData(ICacheManager cacheManager, IUnitOfWork unitOfWork)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
        }

        public User GetById(int userId)
        {
            return cacheManager.Get(string.Format(UserByIdCacheKey, userId),
                () => unitOfWork.UserRepository.GetSingle(user => user.Id == userId),
                cacheExpire);
        }
    }
}