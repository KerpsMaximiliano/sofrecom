using System.Collections.Generic;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;

namespace Sofco.Core.DAL.Common
{
    public interface IUserDelegateRepository
    {
        List<UserDelegate> GetByServiceIds(List<string> serviceIds, UserDelegateType type);

        List<UserDelegate> GetByUserId(int userId, UserDelegateType type);

        UserDelegate Save(UserDelegate userDelegate);

        void Delete(int userDelegateId);

        bool HasUserDelegate(string userName, UserDelegateType type);
    }
}