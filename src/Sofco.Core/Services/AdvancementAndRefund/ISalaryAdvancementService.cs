using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface ISalaryAdvancementService
    {
        Response<IList<SalaryAdvancementModel>> Get();

        Response<SalaryDiscountModel> Add(SalaryDiscountAddModel model);

        Response Delete(int id);
    }
}
