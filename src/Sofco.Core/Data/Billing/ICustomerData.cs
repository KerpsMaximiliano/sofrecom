using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Core.Data.Billing
{
    public interface ICustomerData
    {
        IList<CrmCustomer> GetCustomers(string identityName, string userMail, bool hasDirectorGroup);
    }
}
