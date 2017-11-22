using System.Collections.Generic;
using Sofco.Domain.Crm;

namespace Sofco.Core.Services.Billing
{
    public interface IProjectService
    {
        IList<CrmProjectHito> GetHitosByProject(string projectId);
    }
}