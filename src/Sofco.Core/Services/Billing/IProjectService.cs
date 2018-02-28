using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IProjectService
    {
        IList<CrmProjectHito> GetHitosByProject(string projectId);

        Response<IList<CrmProject>> GetProjects(string serviceId);

        Response<CrmProject> GetProjectById(string projectId);
    }
}