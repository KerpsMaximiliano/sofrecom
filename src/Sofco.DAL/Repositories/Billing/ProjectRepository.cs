using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(SofcoContext context) : base(context)
        {
        }
    }
}
