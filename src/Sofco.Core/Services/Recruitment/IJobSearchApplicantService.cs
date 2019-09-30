using System.Collections.Generic;
using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Recruitment
{
    public interface IJobSearchApplicantService
    {
        Response<IList<JobSearchApplicantModel>> GetByJobSearch(int jobSearchId);
    }
}
