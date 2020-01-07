using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Recruitment
{
    public interface IJobSearchApplicantService
    {
        Response<IList<JobSearchApplicantModel>> GetByJobSearch(int jobSearchId);
        Response Add(JobSearchApplicantAddModel model);
        Response AddInterview(int applicantId, int jobSearchId, DateTime date, InterviewAddModel model);
     
        Response<IList<ApplicantJobSearchModel>> GetByApplicant(int applicantId);
    }
}
