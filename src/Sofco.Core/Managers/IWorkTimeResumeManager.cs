using System;
using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;

namespace Sofco.Core.Managers
{
    public interface IWorkTimeResumeManager
    {
        WorkTimeResumeModel GetResume(List<WorkTimeCalendarModel> calendarModels, DateTime startDate, DateTime endDate);
    }
}