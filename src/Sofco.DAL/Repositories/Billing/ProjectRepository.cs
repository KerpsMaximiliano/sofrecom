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

        public void UpdateInactives(IList<int> idsAdded)
        {
            var projects = context.Projects.Where(x => !idsAdded.Contains(x.Id)).ToList();

            foreach (var project in projects)
            {
                project.Active = false;
                context.Entry(project).Property("Active").IsModified = true;
            }

            context.SaveChanges();
        }
    }
}
