using System.Collections.Generic;
using Sofco.Core.Models.Billing;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISolfacDelegateService
    {
        Response<List<SolfacDelegateModel>> GetAll();

        Response<UserDelegate> Save(UserDelegate userDelegate);

        Response Delete(int userDeletegateId);
    }
}