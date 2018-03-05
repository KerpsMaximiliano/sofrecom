﻿using System;
using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Core.Data.Billing
{
    public interface IServiceData
    {
        IList<CrmService> GetServices(string customerId, string userMail);

        CrmService GetService(Guid serviceId);
    }
}
