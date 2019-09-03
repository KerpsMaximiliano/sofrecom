using Sofco.Core.DAL.Recruitment;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.DAL.Repositories.Recruitment
{
    public class JobSearchRepository : BaseRepository<JobSearch>, IJobSearchRepository
    {
        public JobSearchRepository(SofcoContext context) : base(context)
        {
        }
    }
}
