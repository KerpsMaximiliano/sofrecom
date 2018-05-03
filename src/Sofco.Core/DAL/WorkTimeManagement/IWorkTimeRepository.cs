﻿using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.DAL.WorkTimeManagement
{
    public interface IWorkTimeRepository : IBaseRepository<WorkTime>
    {
        IList<WorkTime> Get(DateTime startDate, DateTime endDate, int currentUserId);
        IList<WorkTime> Search(WorktimeHoursApprovedParams model);
        IList<WorkTime> SearchPending(WorktimeHoursPendingParams model, bool isManagerOrDirector, int currentUserId);
        void UpdateStatus(WorkTime worktime);
        void UpdateApprovalComment(WorkTime worktime);
        void SendHours(int employeeid);
    }
}
