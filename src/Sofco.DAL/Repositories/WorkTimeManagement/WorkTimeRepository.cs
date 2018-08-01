using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Common.Extensions;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;

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

        public IList<WorkTime> SearchApproved(WorktimeHoursApprovedParams parameters)
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

        public IList<WorkTime> SearchPending(WorktimeHoursPendingParams parameters, bool isManagerOrDirector, int currentUserId)
        {
            IQueryable<WorkTime> query1 = context.WorkTimes
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .Where(x => x.Status == WorkTimeStatus.Sent && x.Analytic.ManagerId == currentUserId);

            IQueryable<WorkTime> query2 = from worktime in context.WorkTimes
                                          from worktimeApproval in context.WorkTimeApprovals
                                          where worktime.EmployeeId == worktimeApproval.EmployeeId &&
                                                worktimeApproval.ApprovalUserId == currentUserId &&
                                                worktime.Status == WorkTimeStatus.Sent
                                          select worktime;

            query2 = query2.Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task);

            if (parameters.EmployeeId > 0)
            {
                query1 = query1.Where(x => x.EmployeeId == parameters.EmployeeId);
                query2 = query2.Where(x => x.EmployeeId == parameters.EmployeeId);
            }
                

            if (parameters.AnalyticId > 0)
            {
                query1 = query1.Where(x => x.AnalyticId == parameters.AnalyticId);
                query2 = query2.Where(x => x.AnalyticId == parameters.AnalyticId);
            }

            if (isManagerOrDirector)
            {
                var list1 = query1.ToList();
                var list2 = query2.ToList();
                
                list1.AddRange(list2);

                return list1.DistinctBy(x => x.Id.ToString());
            }
            else
            {
                return query2.ToList();
            }
        }

        public void UpdateStatus(WorkTime worktime)
        {
            context.Entry(worktime).Property("Status").IsModified = true;
            context.Entry(worktime).Property("ApprovalUserId").IsModified = true;
        }

        public void UpdateApprovalComment(WorkTime worktime)
        {
            context.Entry(worktime).Property("ApprovalComment").IsModified = true;
        }

        public void SendHours(int employeeid)
        {
            context.Database.ExecuteSqlCommand($"UPDATE app.worktimes SET status = 2 where status = 1 and employeeid = {employeeid}");
        }

        public void Save(WorkTime workTime)
        {
            if (workTime.Id == 0)
            {
                Insert(workTime);
                return;
            }

            Update(workTime);
        }

        public void RemoveBetweenDays(int licenseEmployeeId, DateTime licenseStartDate, DateTime licenseEndDate)
        {
            context.Database.ExecuteSqlCommand($"DELETE FROM app.worktimes where employeeid = {licenseEmployeeId} AND date >= '{licenseStartDate:yyyy-MM-dd}' and date <= '{licenseEndDate:yyyy-MM-dd}'");
        }

        public decimal GetTotalHoursBetweenDays(int employeeId, DateTime startdate, int analyticId)
        {
            var endDate = new DateTime(startdate.Year, startdate.Month, DateTime.DaysInMonth(startdate.Year, startdate.Month));

            return context.WorkTimes
                .Where(x => x.Date.Date >= startdate.Date && x.Date.Date <= endDate.Date && 
                            x.AnalyticId == analyticId &&
                            (x.Status == WorkTimeStatus.Approved || x.Status == WorkTimeStatus.License) && x.EmployeeId == employeeId)
                .Select(s => s.Hours)
                .Sum();
        }

        public IList<WorkTime> Search(SearchParams parameters)
        {
            IQueryable<WorkTime> query = context.WorkTimes
                .Include(x => x.Employee)
                .Include(x => x.Task)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Analytic)
                    .ThenInclude(x => x.Manager)
                .Where(x => x.Date.Date >= parameters.StartDate.GetValueOrDefault().Date && x.Date.Date <= parameters.EndDate.GetValueOrDefault().Date);

            if (parameters.Status > 0)
                query = query.Where(x => x.Status == (WorkTimeStatus) parameters.Status);

            if (parameters.AnalyticId.HasValue && parameters.AnalyticId > 0)
                query = query.Where(x => x.AnalyticId == parameters.AnalyticId.Value);

            if (parameters.EmployeeId.HasValue && parameters.EmployeeId > 0)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId.Value);

            if (parameters.ManagerId.HasValue && parameters.ManagerId > 0)
                query = query.Where(x => x.Analytic.ManagerId.GetValueOrDefault() == parameters.ManagerId.Value);

            if (!string.IsNullOrWhiteSpace(parameters.ClientId) && !parameters.ClientId.Equals("0"))
                query = query.Where(x => x.Analytic.ClientExternalId == parameters.ClientId);

            return query.ToList();
        }

        public decimal GetPendingHoursByEmployeeId(int employeeId)
        {
            return context.WorkTimes
                .Where(s => s.EmployeeId == employeeId)
                .Select(s => s.Hours)
                .Sum();
        }

        public decimal GetTotalHoursByDate(DateTime date, int currentUserId)
        {
            return context.WorkTimes
                .Where(x => x.UserId == currentUserId
                            && x.Date.Month == date.Month
                            && x.Date.Day == date.Day)
                .Select(s => s.Hours)
                .Sum();
        }

        public decimal GetTotalHoursByDateExceptCurrentId(DateTime date, int currentUserId, int id)
        {
            return context.WorkTimes
                .Where(x => x.UserId == currentUserId 
                        && x.Date.Year == date.Year 
                        && x.Date.Month == date.Month 
                        && x.Date.Day == date.Day
                        && x.Id != id)
                .Select(s => s.Hours)
                .Sum();
        }

        private new void Update(WorkTime workTime)
        {
            var stored = context.WorkTimes.SingleOrDefault(x => x.Id == workTime.Id);

            if (stored == null) throw new Exception("Item Not Found");

            stored.AnalyticId = workTime.AnalyticId;
            stored.TaskId  = workTime.TaskId;
            stored.Date  = workTime.Date;
            stored.Hours  = workTime.Hours;
            stored.UserComment  = workTime.UserComment;
            stored.Status  = workTime.Status;
        }
    }
}
