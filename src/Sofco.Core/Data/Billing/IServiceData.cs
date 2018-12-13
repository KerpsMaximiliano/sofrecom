using System;
using System.Collections.Generic;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Data.Billing
{
    public interface IServiceData
    {
        IList<Service> GetServices(string customerId, string userMail);

        Service GetService(Guid? serviceId);

        void ClearKeys();
    }
}
