using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Validations
{
    public interface IWorkTimeValidation
    {
        void ValidateHours(Response response, WorkTimeAddModel model);
    }
}