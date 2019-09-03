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
    }
}
