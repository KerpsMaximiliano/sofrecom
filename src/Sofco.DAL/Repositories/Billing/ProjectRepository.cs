using System;
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
            var projects = context.Projects.Where(x => x.ServiceId.Equals(serviceId) && x.Active).ToList();

            foreach (var project in projects)
            {
                var opp = context.Opportunities.SingleOrDefault(x => x.CrmId == project.OpportunityId);

                if (opp != null)
                {
                    var contact = context.Contacts.SingleOrDefault(x => x.CrmId == opp.ContactId);

                    if (contact != null)
                    {
                        project.PrincipalContactName = contact.Name;
                        project.PrincipalContactEmail = contact.Email;
                    }
                }
            }

            return projects;
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
