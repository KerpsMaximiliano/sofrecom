using System.Collections.Generic;
using Sofco.Domain.Crm;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmInvoicingMilestoneService
    {
        List<CrmHito> GetToExpire(int daysToExpire);
    }
}