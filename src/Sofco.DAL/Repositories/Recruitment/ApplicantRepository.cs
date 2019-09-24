using Sofco.Core.DAL.Recruitment;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.DAL.Repositories.Recruitment
{
    public class ApplicantRepository : BaseRepository<Applicant>, IApplicantRepository
    {
        public ApplicantRepository(SofcoContext context) : base(context)
        {
        }
    }
}
