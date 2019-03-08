using System.Linq;
using Sofco.Core.DAL.Workflow;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Repositories.Workflow
{
    public class UserSourceRepository : IUserSourceRepository
    {
        protected readonly SofcoContext context;

        public UserSourceRepository(SofcoContext context)
        {
            this.context = context;
        }

        public UserSource Get(string managerUserSource, int sourceId)
        {
            return context.UserSources.SingleOrDefault(x => x.Code == managerUserSource && x.SourceId == sourceId);
        }

        public void Add(UserSource userSource)
        {
            context.UserSources.Add(userSource);
        }
    }
}
