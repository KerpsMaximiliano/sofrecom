using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(SofcoContext context) : base(context)
        {
        }

        public Project GetByIdCrm(string crmProjectId)
        {
            return context.Projects.SingleOrDefault(x => x.CrmId.Equals(crmProjectId));
        }

        public IList<Project> GetAllActives(string serviceId)
        {
            return context.Projects.Where(x => x.ServiceId.Equals(serviceId)).ToList();
        }
    }
}
