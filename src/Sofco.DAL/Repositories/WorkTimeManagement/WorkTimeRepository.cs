using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Enums;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.DAL.Repositories.WorkTimeManagement
{
    public class WorkTimeRepository : BaseRepository<WorkTime>, IWorkTimeRepository
    {
        public WorkTimeRepository(SofcoContext context) : base(context)
        {
        }

        public IList<WorkTime> Get(DateTime startDate, DateTime endDate, int currentUserId)
        {
            return context.WorkTimes
                .Where(x => x.UserId == currentUserId && x.Date >= startDate && x.Date <= endDate)
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .ToList();
        }

        public IList<WorkTime> Search(WorktimeHoursApprovedParams parameters)
        {
            IQueryable<WorkTime> query = context.WorkTimes.Include(x => x.Employee).Include(x => x.Analytic).Include(x => x.Task).Where(x => x.Status == WorkTimeStatus.Approved || x.Status == WorkTimeStatus.License);

            if (parameters.EmployeeId > 0)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId);

            if (parameters.AnalyticId > 0)
                query = query.Where(x => x.AnalyticId == parameters.AnalyticId);

            if (parameters.Month.HasValue && parameters.Month > 0 && parameters.Year.HasValue && parameters.Year > 0)
            {
                var startDate = new DateTime(parameters.Year.Value, parameters.Month.Value, 1);
                var endDate = new DateTime(parameters.Year.Value, parameters.Month.Value, DateTime.DaysInMonth(parameters.Year.Value, parameters.Month.Value));

                query = query.Where(x => x.Date >= startDate && x.Date <= endDate);
            }

            return query.ToList();
        }

        public IList<WorkTime> SearchPending(WorktimeHoursPendingParams parameters)
        {
            IQueryable<WorkTime> query = context.WorkTimes.Include(x => x.Employee).Include(x => x.Analytic).Include(x => x.Task).Where(x => x.Status == WorkTimeStatus.Sent);

            if (parameters.EmployeeId > 0)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId);

            if (parameters.AnalyticId > 0)
                query = query.Where(x => x.AnalyticId == parameters.AnalyticId);

            return query.ToList();
        }
    }
}
