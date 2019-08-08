using System.Collections.Generic;
using Sofco.Core.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Common
{
    public interface IDelegationService
    {
        Response Add(DelegationAddModel model);
        Response Delete(int id);
        Response<IList<DelegationModel>> GetByUserId();
        Response<IList<AnalyticsWithEmployees>> GetAnalytics();
    }
}
