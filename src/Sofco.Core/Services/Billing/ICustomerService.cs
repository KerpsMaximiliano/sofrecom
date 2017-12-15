﻿using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ICustomerService
    {
        IList<CrmCustomer> GetCustomers(string userMail, string identityName);
        Response<CrmCustomer> GetCustomerById(string customerId);
    }
}
