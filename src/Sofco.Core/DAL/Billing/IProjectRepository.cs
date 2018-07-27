using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Project GetByIdCrm(string crmProjectId);
        IList<Project> GetAllActives(string serviceId);
    }
}
