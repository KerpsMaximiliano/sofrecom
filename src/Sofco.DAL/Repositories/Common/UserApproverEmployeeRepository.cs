using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Repositories.Common
{
    public class UserApproverEmployeeRepository : BaseRepository<Employee>, IUserApproverEmployeeRepository
    {
        private UserApproverQuery parameter;

        public UserApproverEmployeeRepository(SofcoContext context) : base(context)
        {
            parameter = new UserApproverQuery();
        }

        public List<UserApproverEmployee> GetByAllocations(UserApproverQuery query, UserApproverType type)
        {
            var date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            parameter = query;

            Expression<Func<Employee, bool>> where = x => x.EndDate == null 
                && x.Allocations.Any(s => s.StartDate.Date >= date.Date
                                          && s.Percentage > 0);

            if (query.AnalyticId > 0)
            {
                where = e => e.EndDate == null
                    && e.Allocations.Any(s => 
                        s.AnalyticId == query.AnalyticId
                        && s.StartDate.Date >= date.Date
                        && s.Percentage > 0);
            }

            var result = context
                .Employees
                .Where(where)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .GroupJoin(context.UserApprovers.Where(s => s.Type == type).ToList(),
                    e => e.Id,
                    w => w.EmployeeId,
                    Translate);

            if (query.ApprovalId > 0)
            {
                result = result.Where(s =>
                    s.UserApprover != null && s.UserApprover.ApproverUserId == query.ApprovalId);
            }

            return result.ToList();
        }

        public List<UserApproverEmployee> GetByAllocationAnalytics(List<int> analyticIds, int approvalId, UserApproverType type)
        {
            var date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            Expression<Func<Employee, bool>> where = e => e.EndDate == null 
                && e.Allocations.Any(x => analyticIds.Contains(x.AnalyticId)
                && x.StartDate >= date.Date
                && x.Percentage > 0);

            var result = context
                .Employees
                .Where(where)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .GroupJoin(
                    context.UserApprovers.Where(s => s.Type == type).ToList(),
                    e => e.Id,
                    w => w.EmployeeId,
                    Translate);

            if (approvalId > 0)
            {
                result = result.Where(s =>
                    s.UserApprover != null && s.UserApprover.ApproverUserId == approvalId);
            }

            return result.ToList();
        }

        public List<UserApproverEmployee> GetByAllocationManagersBySectors(List<int> sectorIds, UserApproverType type)
        {
            var date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var managerIds = context.Analytics.Where(s => sectorIds.Contains(s.SectorId) && s.ManagerId.HasValue)
                .Select(s => s.ManagerId)
                .Distinct()
                .ToList();

            var userEmails = context.Users.Where(s => managerIds.Contains(s.Id)).Select(s => s.Email).ToList();

            Expression<Func<Employee, bool>> where = e => e.EndDate == null
                && userEmails.Contains(e.Email)
                && e.Allocations.Any(x => x.StartDate >= date.Date && x.Percentage > 0);

            var result = context
                .Employees
                .Where(where)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .GroupJoin(
                    context.UserApprovers.Where(s => s.Type == type).ToList(),
                    e => e.Id,
                    w => w.EmployeeId,
                    Translate);

            return result.ToList();
        }

        public UserApproverEmployee Translate(Employee employee, IEnumerable<UserApprover> userApprovers)
        {
            var date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var allocation = employee.Allocations.FirstOrDefault(s => s.StartDate >= date.Date);
            if (parameter.AnalyticId > 0)
            {
                allocation = employee.Allocations.FirstOrDefault(s =>
                    s.AnalyticId == parameter.AnalyticId && s.StartDate >= date.Date);
            }

            var analyticId = parameter.AnalyticId > 0 ? parameter.AnalyticId : allocation?.AnalyticId;

            var firstUserApprover = userApprovers.FirstOrDefault(s => s.AnalyticId == analyticId);

            return new UserApproverEmployee
            {
                EmployeeId = employee.Id.ToString(),
                Name = employee.Name,
                Client = allocation?.Analytic?.AccountName,
                Service = allocation?.Analytic?.ServiceName,
                ManagerId = allocation?.Analytic?.ManagerId,
                ApprovalName = firstUserApprover?.ApproverUser?.Name,
                ClientId = allocation?.Analytic?.AccountId,
                AnalyticId = allocation?.AnalyticId,
                UserApprover = firstUserApprover
            };
        }
    }
}
