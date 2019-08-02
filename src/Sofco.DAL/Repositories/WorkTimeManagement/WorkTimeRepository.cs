using System;
using System.Collections.Generic;
using System.Linq;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Sofco.Common.Extensions;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
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
                .ThenInclude(x => x.Category)
                .ToList();
        }

        public IList<WorkTime> GetByEmployeeId(DateTime startDate, DateTime endDate, int employeeId)
        {
            return context.WorkTimes
                .Where(x => x.EmployeeId == employeeId 
                            && x.Date.Date >= startDate.Date 
                            && x.Date.Date <= endDate.Date)
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .ToList();
        }

        public IList<WorkTime> GetByEmployeeIds(DateTime startDate, DateTime endDate, List<int> employeeIds)
        {
            return context.WorkTimes
                .Where(x => employeeIds.Contains(x.EmployeeId)
                            && x.Date.Date >= startDate.Date
                            && x.Date.Date <= endDate.Date)
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .ToList();
        }

        public IList<WorkTime> GetByEmployeeIds(List<int> employeeIds)
        {
            return context.WorkTimes
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .ToList();
        }

        public IList<WorkTime> GetByEmployeeIds(DateTime startDate, DateTime endDate, IList<int> employeeIds)
        {
            return context.WorkTimes
                .Where(x => employeeIds.Contains(x.EmployeeId)
                            && x.Date.Date >= startDate.Date
                            && x.Date.Date <= endDate.Date)
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .ToList();
        }

        public IList<WorkTime> GetByEmployeeIds(IList<int> employeeIds)
        {
            return context.WorkTimes
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .ToList();
        }

        public IList<WorkTime> GetByAnalyticIds(DateTime startDate, DateTime endDate, List<int> analyticIds)
        {
            return context.WorkTimes
                .Where(x => analyticIds.Contains(x.AnalyticId) && x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date)
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .ToList();
        }

        public IList<WorkTime> SearchApproved(WorktimeHoursApprovedParams parameters)
        {
            var query = context.WorkTimes.Include(x => x.Employee).Include(x => x.Analytic).Include(x => x.Task).Where(x => x.Status == WorkTimeStatus.Approved || x.Status == WorkTimeStatus.License);

            if (parameters.EmployeeId > 0)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId);

            query = query.Where(x => parameters.AnalyticIds.Contains(x.AnalyticId));

            if (parameters.FilterByDates)
            {
                query = query.Where(x => x.Date.Date >= parameters.StartDate.GetValueOrDefault().Date && x.Date.Date <= parameters.EndDate.GetValueOrDefault().Date);
            }

            return query.ToList();
        }

        public IList<WorkTime> SearchPending(WorktimeHoursPendingParams parameters, bool isManagerOrDirector,
            int currentUserId, string analyticBank)
        {
            IQueryable<WorkTime> query1 = context.WorkTimes
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .Where(x => (x.Status == WorkTimeStatus.Sent || (x.Status == WorkTimeStatus.License && x.Analytic.Title.Equals(analyticBank))) && x.Analytic.ManagerId == currentUserId);

            IQueryable<WorkTime> query2 = from worktime in context.WorkTimes
                                          from userApprover in context.UserApprovers
                                          where 
                                            worktime.EmployeeId == userApprover.EmployeeId
                                            && worktime.Status == WorkTimeStatus.Sent
                                            && userApprover.ApproverUserId == currentUserId
                                            && userApprover.AnalyticId == worktime.AnalyticId 
                                            && userApprover.Type == UserApproverType.WorkTime
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

        public void SendHours(int employeeId)
        {
            context.Database.ExecuteSqlCommand($"UPDATE app.worktimes SET status = 2 where status = 1 and employeeid = {employeeId}");
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
            var worktimes = context.WorkTimes.Where(x =>
                x.EmployeeId == licenseEmployeeId && x.Date.Date >= licenseStartDate.Date &&
                x.Date.Date <= licenseEndDate.Date).ToList();

            Delete(worktimes);
        }

        public decimal GetTotalHoursBetweenDays(int employeeId, DateTime startdate, DateTime endDate, int analyticId)
        {
            return context.WorkTimes
                .Where(x => x.Date.Date >= startdate.Date && x.Date.Date <= endDate.Date && 
                            x.AnalyticId == analyticId && x.EmployeeId == employeeId)
                .Select(s => s.Hours)
                .Sum();
        }

        public decimal GetTotalHoursApprovedBetweenDays(int employeeId, DateTime startdate, DateTime endDate, int analyticId)
        {
            return context.WorkTimes
                .Where(x => x.Date.Date >= startdate.Date && x.Date.Date <= endDate.Date &&
                            x.AnalyticId == analyticId &&
                            (x.Status == WorkTimeStatus.Approved || x.Status == WorkTimeStatus.License) && x.EmployeeId == employeeId)
                .Select(s => s.Hours)
                .Sum();
        }

        public List<WorkTime> GetWorkTimePendingHoursByEmployeeId(int employeeId)
        {
            return context.WorkTimes
                .Include(x => x.Task)
                .Where(s => s.EmployeeId == employeeId
                            && s.Status == WorkTimeStatus.Sent)
                .ToList();
        }

        public IList<WorkTime> Search(SearchParams parameters, List<int> analyticIds)
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

            if (parameters.AnalyticId.Any())
                query = query.Where(x => parameters.AnalyticId.Contains(x.AnalyticId));
            else
            {
                query = query.Where(x => analyticIds.Contains(x.AnalyticId));
            }

            if (parameters.EmployeeId.HasValue && parameters.EmployeeId > 0)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId.Value);

            if (parameters.ManagerId.Any())
                query = query.Where(x => parameters.ManagerId.Contains(x.Analytic.ManagerId.GetValueOrDefault()));

            return query.ToList();
        }

        public void InsertBulk(IList<WorkTime> workTimesToAdd)
        {
            context.BulkInsert(workTimesToAdd);
        }

        public void SendManagerHours(int employeeid, int analyticId)
        {
            context.Database.ExecuteSqlCommand($"UPDATE app.worktimes SET status = 3 where status = 1 and employeeid = {employeeid} and analyticId = {analyticId}");
        }

        public List<WorkTime> GetWorkTimeDraftByEmployeeId(int employeeId)
        {
            return context.WorkTimes
                .Include(x => x.Task)
                .Where(s => s.EmployeeId == employeeId
                            && s.Status == WorkTimeStatus.Draft)
                .ToList();
        }

        public IList<WorkTime> GetByDate(DateTime worktimeDate)
        {
            return context.WorkTimes.Where(x => x.Date.Date == worktimeDate.Date).ToList();
        }

        public decimal GetPendingHoursByEmployeeId(int employeeId)
        {
            return context.WorkTimes
                .Where(s => s.EmployeeId == employeeId
                            && s.Status == WorkTimeStatus.Sent)
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
            stored.Reference = workTime.Reference;
        }

        //Elimina todas las horas cargadas para una fecha determinada
        public void RemoveAllOfDate(DateTime removeDate)
        {
            var worktimes = context.WorkTimes.Where(x => x.Date.Date == removeDate.Date).ToList();

            Delete(worktimes);
        }

        public void SendHours(int employeeId, int analyticId)
        {
            context.Database.ExecuteSqlCommand($"UPDATE app.worktimes SET status = 2 where status = 1 and employeeid = {employeeId} and analyticId = {analyticId}");
        }

        public IList<Analytic> GetAnalyticsToApproveHours(int currentEmployeeId)
        {
            return context.WorkTimes.Include(x => x.Analytic)
                .Where(x => x.EmployeeId == currentEmployeeId && x.Status == WorkTimeStatus.Draft).Select(x => x.Analytic).Distinct().ToList();
        }
    }
}
