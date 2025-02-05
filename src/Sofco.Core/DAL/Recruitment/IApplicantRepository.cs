﻿using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.DAL.Recruitment
{
    public interface IApplicantRepository : IBaseRepository<Applicant>
    {
        IList<Applicant> Search(ApplicantSearchParameters parameter);
        Applicant GetDetail(int id);
        IList<JobSearchApplicant> GetHistory(int applicantId);
        void AddHistory(ApplicantHistory history);
        IList<ApplicantFile> GetFiles(int id);
        Applicant GetWithProfilesAndSkills(int applicantId);
    }
}
