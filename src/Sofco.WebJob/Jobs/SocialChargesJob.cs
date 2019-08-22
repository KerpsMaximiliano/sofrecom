using System;
using Sofco.Core.Services.Rrhh;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class SocialChargesJob : ISocialChargesJob
    {
        private readonly IRrhhService rrhhService;

        public SocialChargesJob(IRrhhService rrhhService)
        {
            this.rrhhService = rrhhService;
        }

        public void Execute()
        {
            var now = DateTime.UtcNow;
            rrhhService.UpdateSocialCharges(now.Year, now.Month);
        }
    }
}
