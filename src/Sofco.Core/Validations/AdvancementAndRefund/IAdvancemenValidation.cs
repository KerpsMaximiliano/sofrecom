using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.AdvancementAndRefund
{
    public interface IAdvancemenValidation
    {
        Response ValidateAdd(AdvancementModel model);
        Response ValidateUpdate(AdvancementModel model);
    }
}
