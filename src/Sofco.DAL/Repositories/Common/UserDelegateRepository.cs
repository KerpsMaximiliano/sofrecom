using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;

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

        public UserDelegate Save(UserDelegate userDelegate)
        {
            var storedItem = GetByServiceIdAndUserId(userDelegate.ServiceId, userDelegate.UserId, userDelegate.Type);

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
                .Any(s => 
                s.UserId == context.Users.Single(x => x.UserName == userName).Id
                && s.Type == type);
        }

        private UserDelegate GetByServiceIdAndUserId(Guid serviceId, int userId, UserDelegateType type)
        {
            return UserDelegateSet
                .SingleOrDefault(s => s.ServiceId == serviceId 
                && s.UserId == userId
                && s.Type == type);
        }
    }
}
