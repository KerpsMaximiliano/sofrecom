﻿using System.Collections.Generic;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Data.Billing
{
    public interface ICustomerData
    {
        IList<Customer> GetCustomers(string username);

        void ClearKeys();

        void ClearKeys(string username);
    }
}
