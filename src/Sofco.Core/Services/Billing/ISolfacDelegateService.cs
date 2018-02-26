using System;
using System.Collections.Generic;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISolfacDelegateService
    {
        Response<List<SolfacDelegate>> GetByServiceId(Guid serviceId);

        Response<SolfacDelegate> Save(SolfacDelegate solfacDelegate);
    }
}