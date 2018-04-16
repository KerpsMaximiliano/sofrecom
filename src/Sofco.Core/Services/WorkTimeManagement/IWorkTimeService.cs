using System;
using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.WorkTimeManagement
{
    public interface IWorkTimeService
    {
        IList<WorkTimeModel> Get(DateTime date);
        Response<WorkTimeAddModel> Add(WorkTimeAddModel model);
    }
}
