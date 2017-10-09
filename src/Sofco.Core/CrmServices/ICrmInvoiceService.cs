using Sofco.Common.Domains;
using Sofco.Domain.Crm;
using System.Collections.Generic;

namespace Sofco.Core.CrmServices
{
    public interface ICrmInvoiceService
    {
        Result<List<CrmHito>> Get(int daysToExpire);
    }
}
