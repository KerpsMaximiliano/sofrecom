using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.DAL.Recruitment
{
    public interface IJobSearchApplicantRepository : IBaseRepository<JobSearchApplicant>
    {
        IList<Applicant> Get(IList<int> skills, IList<int> profiles);
        JobSearchApplicant GetById(int applicantId, int jobSearchId, DateTime date);
        void InsertFile(JobSearchApplicantFile jobsearchApplicantFile);
        bool Exist(int applicantId, int jobSearchId, DateTime date);
    }
}
