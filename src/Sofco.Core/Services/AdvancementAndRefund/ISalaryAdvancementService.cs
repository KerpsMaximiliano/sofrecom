using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface ISalaryAdvancementService
    {
        Response<IList<SalaryAdvancementModel>> Get();

        void Import(IFormFile file, Response response);
    }
}
