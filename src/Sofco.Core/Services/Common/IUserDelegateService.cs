using System.Collections.Generic;
using Sofco.Core.Models.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Common
{
    public interface IUserDelegateService
    {
        Response<List<UserDelegateModel>> GetAll(UserDelegateType type);

        Response<UserDelegate> Save(UserDelegate userDelegate);

        Response Delete(int userDeletegateId);
    }
}