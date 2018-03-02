using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Core.Data.Billing
{
    public interface IProjectData
    {
        IList<CrmProject> GetProjects(string serviceId, string userMail);

        IList<CrmProjectHito> GetHitos(string projectId);
    }
}
