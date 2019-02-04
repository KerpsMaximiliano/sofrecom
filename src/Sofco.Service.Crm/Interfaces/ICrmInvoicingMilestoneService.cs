using System;
using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmInvoicingMilestoneService
    {
        List<CrmHito> GetToExpire(int daysToExpire);

        List<CrmProjectHito> GetByProjectId(Guid projectId);

        void Update(HitoSplittedParams data, Response response);

        void Create(HitoSplittedParams data, Response response);

        void Close(Response response, string id, string status);
    }
}