using System.Collections.Generic;
using Sofco.Core.Models.Billing;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISolfacDelegateService
    {
        Response<List<SolfacDelegateModel>> GetAll();

        Response<SolfacDelegate> Save(SolfacDelegate solfacDelegate);

        Response Delete(int solfacDeletegateId);
    }
}