﻿using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Customer GetByIdCrm(string crmCustomerId);
        IList<Customer> GetAllByServices(IEnumerable<string> servicesIds);
        IList<Customer> GetAllActives();
        void UpdateInactives(IList<int> idsAdded);
    }
}
