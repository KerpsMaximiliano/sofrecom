using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IProjectService
    {
        IList<CrmProjectHito> GetHitosByProject(string projectId, bool reload);
        IList<CrmProject> GetProjects(string serviceId, string getUserMail, string getUserName);
        Response<CrmProject> GetProjectById(string projectId);
    }
}