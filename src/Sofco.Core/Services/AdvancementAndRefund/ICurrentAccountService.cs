using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface ICurrentAccountService
    {
        Response<IList<CurrentAccountModel>> Get();
        Response UpdateMassive(UpdateMassiveModel model);
    }
}
