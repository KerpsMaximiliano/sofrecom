using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Repositories.Common
{
    public class UserDelegateRepository : BaseRepository<UserDelegate>, IUserDelegateRepository
    {
        private DbSet<UserDelegate> UserDelegateSet { get; }

        public UserDelegateRepository(SofcoContext context) : base(context)
        {
            UserDelegateSet = context.Set<UserDelegate>();
        }

        public List<UserDelegate> GetByServiceIds(List<string> serviceIds, UserDelegateType type)
        {
            return UserDelegateSet
                .Where(s => serviceIds.Contains(s.ServiceId.ToString()) 
                && s.Type == type)
                .ToList();
        }

        public List<UserDelegate> GetByUserId(int userId, UserDelegateType type)
        {
            return UserDelegateSet
                .Where(s => s.UserId == userId && s.Type == type)
                .ToList();
        }

        public List<UserDelegate> GetByUserId(int userId, List<UserDelegateType> types)
        {
            return UserDelegateSet
                .Where(s => s.UserId == userId && types.Contains(s.Type))
                .ToList();
        }

        public UserDelegate Save(UserDelegate userDelegate)
        {
            var storedItem = GetStored(userDelegate);

            if (storedItem != null)
            {
                Update(storedItem);
            }
            else
            {
                Insert(userDelegate);
            }

            context.SaveChanges();

            return userDelegate;
        }

        public void Delete(int userDelegateId)
        {
            var userDelegate = new UserDelegate { Id = userDelegateId };

            context.Entry(userDelegate).State = EntityState.Deleted;

            context.SaveChanges();
        }

        public bool HasUserDelegate(string userName, UserDelegateType type)
        {
            return UserDelegateSet
                .Any(s => s.UserId == context.Users.Single(x => x.UserName == userName).Id
                && s.Type == type);
        }

        public bool HasUserDelegate(string userName, List<UserDelegateType> types)
        {
            return UserDelegateSet
                .Any(s => s.UserId == context.Users.Single(x => x.UserName == userName).Id
                && types.Contains(s.Type));
        }

        public List<UserDelegate> GetByTypesAndSourceId(List<UserDelegateType> types, int sourceId)
        {
            return UserDelegateSet
                .Where(s => types.Contains(s.Type) && s.SourceId == sourceId)
                .ToList();
        }

        public List<UserDelegate> GetByTypeAndSourceId(UserDelegateType type, int sourceId)
        {
            return UserDelegateSet
                .Where(s => s.Type == type && s.SourceId == sourceId)
                .ToList();
        }

        public IList<UserDelegate> GetByUserIdAndType(int currentUserId, UserDelegateType type)
        {
            return UserDelegateSet
                .Where(s => s.Type == type && s.UserId == currentUserId)
                .ToList();
        }

        private UserDelegate GetStored(UserDelegate userDelegate)
        {
            if (userDelegate.Type == UserDelegateType.Solfac
                || userDelegate.Type == UserDelegateType.LicenseView
                || userDelegate.Type == UserDelegateType.PurchaseOrderActive)
            {
                return GetByServiceIdAndUserId(userDelegate.ServiceId, userDelegate.UserId, userDelegate.Type);
            }

            return GetBySourceIdAndUserId(userDelegate.SourceId, userDelegate.UserId, userDelegate.Type);
        }

        private UserDelegate GetByServiceIdAndUserId(Guid? serviceId, int userId, UserDelegateType type)
        {
            return UserDelegateSet
                .SingleOrDefault(s => s.ServiceId == serviceId 
                && s.UserId == userId
                && s.Type == type);
        }

        private UserDelegate GetBySourceIdAndUserId(int? sourceId, int userId, UserDelegateType type)
        {
            return UserDelegateSet
                .SingleOrDefault(s => s.SourceId == sourceId
                                      && s.UserId == userId
                                      && s.Type == type);
        }
    }
}
