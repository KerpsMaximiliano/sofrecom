using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.AdvancementAndRefund
{
    public interface IAdvancemenValidation
    {
        void ValidateAdd(AdvancementModel model, Response response);
        Response ValidateUpdate(AdvancementModel model);
    }
}
