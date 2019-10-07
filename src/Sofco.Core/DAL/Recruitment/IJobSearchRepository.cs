using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.DAL.Recruitment
{
    public interface IJobSearchRepository : IBaseRepository<JobSearch>
    {
        IList<JobSearch> Search(JobSearchParameter parameter);
        JobSearch GetDetail(int id);
        JobSearch GetWithProfilesAndSkills(int jobSearchId);
        JobSearch GetWithProfilesAndSkillsAndSenorities(int id);
    }
}
