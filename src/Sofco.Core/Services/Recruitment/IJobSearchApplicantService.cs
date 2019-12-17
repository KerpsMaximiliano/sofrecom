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
        Response AddInterview(int applicantId, int jobSearchId, InterviewAddModel model);
        Task<Response<File>> AttachFile(int applicantId, int jobSearchId, Response<File> response,
            IFormFile file);
    }
}
