using System.Collections.Generic;
using Sofco.Core.Models.Common;
using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Recruitment
{
    public interface IJobSearchService
    {
        Response Add(JobSearchAddModel model);
        Response<IList<OptionModel>> GetApplicants();
        Response<IList<OptionModel>> GetRecruiters();
        Response<IList<JobSearchResultModel>> Search(JobSearchParameter parameter);
        Response<JobSearchModel> Get(int id);
        Response Update(int id, JobSearchAddModel model);
        Response ChangeStatus(int id, JobSearchChangeStatusModel parameter);
    }
}
