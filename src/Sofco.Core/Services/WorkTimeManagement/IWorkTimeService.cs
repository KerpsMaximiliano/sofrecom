using System;
using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.WorkTimeManagement
{
    public interface IWorkTimeService
    {
        Response<WorkTimeModel> Get(DateTime date);

        Response<WorkTime> Add(WorkTimeAddModel model);

        Response<IList<HoursApprovedModel>> GetHoursApproved(WorktimeHoursApprovedParams model);
    }
}
