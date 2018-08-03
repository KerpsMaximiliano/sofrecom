using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.Common
{
    public interface IUserDelegateRepository
    {
        List<UserDelegate> GetByServiceIds(List<string> serviceIds, UserDelegateType type);

        List<UserDelegate> GetByUserId(int userId, UserDelegateType type);

        List<UserDelegate> GetByUserId(int userId, List<UserDelegateType> types);

        UserDelegate Save(UserDelegate userDelegate);

        void Delete(int userDelegateId);

        bool HasUserDelegate(string userName, UserDelegateType type);

        bool HasUserDelegate(string userName, List<UserDelegateType> types);

        List<UserDelegate> GetByTypesAndSourceId(List<UserDelegateType> types, int sourceId);

        List<UserDelegate> GetByTypeAndSourceId(UserDelegateType type, int sourceId);
    }
}