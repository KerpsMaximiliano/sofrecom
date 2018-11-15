using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations
{
    public interface IWorkTimeValidation
    {
        void ValidateDate(Response response, WorkTimeAddModel model);

        void ValidateDelete(Response response, WorkTime workTime);

        void ValidateHours(Response response, WorkTimeAddModel model);

        void ValidateAllocations(Response response, WorkTimeAddModel model);

        void ValidateAllocations(Response response, List<WorkTime> workTimes);
    }
}