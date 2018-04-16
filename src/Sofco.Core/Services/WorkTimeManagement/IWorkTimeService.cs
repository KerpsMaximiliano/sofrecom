using System;
using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;

namespace Sofco.Core.Services.WorkTimeManagement
{
    public interface IWorkTimeService
    {
        IList<WorkTimeModel> Get(DateTime date);
    }
}
