using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Data.Billing
{
    public interface IProjectData
    {
        IList<Project> GetProjects(string serviceId);

        IList<CrmProjectHito> GetHitos(string projectId);
        void ClearKeys();
    }
}
