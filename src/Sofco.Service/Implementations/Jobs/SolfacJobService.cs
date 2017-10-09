using Sofco.Common.Domains;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Jobs;

namespace Sofco.Service.Implementations.Jobs
{
    public class SolfacJobService : ISolfacJobService
    {
        private readonly ISolfacRepository solfacRepository;

        public SolfacJobService(ISolfacRepository solfacRepository)
        {
            this.solfacRepository = solfacRepository;
        }

        public Result Get()
        {
            var solfacs = solfacRepository.GetAll();

            return new Result(solfacs);
        }
    }
}
