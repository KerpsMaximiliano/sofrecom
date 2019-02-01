using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Crm;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmInvoicingMilestoneService
    {
        List<CrmHito> GetToExpire(int daysToExpire);

        List<CrmProjectHito> GetByProjectId(Guid projectId);

        Result Update(CrmInvoicingMilestone data);

        Result Create(CrmInvoicingMilestone data);
    }
}