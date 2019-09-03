using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Recruitment
{
    public interface IJobSearchService
    {
        Response Add(JobSearchAddModel model);
    }
}
