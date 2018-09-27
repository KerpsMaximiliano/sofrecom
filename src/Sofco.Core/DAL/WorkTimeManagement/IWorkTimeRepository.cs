using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.DAL.WorkTimeManagement
{
    public interface IWorkTimeRepository : IBaseRepository<WorkTime>
    {
        IList<WorkTime> Get(DateTime startDate, DateTime endDate, int currentUserId);

        IList<WorkTime> GetByAnalyticIds(DateTime startDate, DateTime endDate, List<int> analyticIds);

        IList<WorkTime> SearchApproved(WorktimeHoursApprovedParams model);

        decimal GetTotalHoursByDateExceptCurrentId(DateTime date, int currentUserId, int id);

        IList<WorkTime> SearchPending(WorktimeHoursPendingParams model, bool isManagerOrDirector, int currentUserId);

        void UpdateStatus(WorkTime worktime);

        void UpdateApprovalComment(WorkTime worktime);

        void SendHours(int employeeId);

        void Save(WorkTime workTime);

        void RemoveBetweenDays(int licenseEmployeeId, DateTime licenseStartDate, DateTime licenseEndDate);

        decimal GetTotalHoursBetweenDays(int allocationEmployeeId, DateTime startDate, DateTime endDate, int analyticId);

        decimal GetPendingHoursByEmployeeId(int employeeId);

        List<WorkTime> GetWorkTimePendingHoursByEmployeeId(int employeeId);

        IList<WorkTime> Search(SearchParams parameters);

        void InsertBulk(IList<WorkTime> workTimesToAdd);

        void SendManagerHours(int id);
    }
}
