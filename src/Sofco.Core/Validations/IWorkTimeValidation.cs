using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations
{
    public interface IWorkTimeValidation
    {
        void ValidateHours(Response response, WorkTimeAddModel model);
    }
}