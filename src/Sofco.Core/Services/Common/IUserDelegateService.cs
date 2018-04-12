using System.Collections.Generic;
using Sofco.Core.Models.Common;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Common
{
    public interface IUserDelegateService
    {
        Response<List<UserDelegateModel>> GetAll(UserDelegateType type);

        Response<UserDelegate> Save(UserDelegate userDelegate);

        Response Delete(int userDeletegateId);
    }
}