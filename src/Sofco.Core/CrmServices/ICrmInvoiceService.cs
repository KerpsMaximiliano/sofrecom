using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Crm;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.CrmServices
{
    public interface ICrmInvoiceService
    {
        Result<List<CrmHito>> GetHitosToExpire(int daysToExpire);

        Result<string> CreateHitoBySolfac(Solfac solfac);
    }
}
