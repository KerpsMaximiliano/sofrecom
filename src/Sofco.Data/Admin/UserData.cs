using System;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Cache;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Models.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Data.Admin
{
    public class UserData : IUserData
    {
        private const string UserByIdCacheKey = "urn:users:id:{0}";
        private const string UserByUserNamCacheKey = "urn:users:userName:{0}";
        private const string UserByMangerIdCacheKey = "urn:users:managerId:{0}";
        private const string UserLiteByMangerIdCacheKey = "urn:userLites:id:{0}";
        private const string UserLiteByUserNameCacheKey = "urn:userLites:userName:{0}";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly ISessionManager sessionManager;

        private readonly IUnitOfWork unitOfWork;

        public UserData(ICacheManager cacheManager, IUnitOfWork unitOfWork, ISessionManager sessionManager)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
        }

        public User GetByExternalManagerId(string managerId)
        {
            return cacheManager.Get(string.Format(UserByMangerIdCacheKey, managerId),
                () => unitOfWork.UserRepository.GetSingle(user => user.ExternalManagerId == managerId),
                cacheExpire);
        }

        public User GetById(int userId)
        {
            return cacheManager.Get(string.Format(UserByIdCacheKey, userId),
                () => unitOfWork.UserRepository.GetSingle(user => user.Id == userId),
                cacheExpire);
        }

        public User GetByUserName(string userName)
        {
            return cacheManager.Get(string.Format(UserByUserNamCacheKey, userName),
                () => unitOfWork.UserRepository.GetSingle(user => user.UserName == userName),
                cacheExpire);
        }

        public UserLiteModel GetUserLiteById(int userId)
        {
            return cacheManager.Get(string.Format(UserLiteByMangerIdCacheKey, userId),
                () => unitOfWork.UserRepository.GetUserLiteById(userId),
                cacheExpire);
        }

        public UserLiteModel GetUserLiteByUserName(string userName)
        {
            return cacheManager.Get(string.Format(UserLiteByUserNameCacheKey, userName),
                () => unitOfWork.UserRepository.GetUserLiteByUserName(userName),
                cacheExpire);
        }

        public UserLiteModel GetCurrentUser()
        {
            var currentUserName = sessionManager.GetUserName();

            return GetUserLiteByUserName(currentUserName);
        }
    }
}