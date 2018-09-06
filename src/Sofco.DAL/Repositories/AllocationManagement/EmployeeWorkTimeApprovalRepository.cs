using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeWorkTimeApprovalRepository : BaseRepository<Employee>, IEmployeeWorkTimeApprovalRepository
    {
        private WorkTimeApprovalQuery parameter;

        public EmployeeWorkTimeApprovalRepository(SofcoContext context) : base(context)
        {
            parameter = new WorkTimeApprovalQuery();
        }

        public List<WorkTimeApprovalEmployee> Get(WorkTimeApprovalQuery query)
        {
            parameter = query;

            Expression<Func<Employee, bool>> where = x => x.EndDate == null 
                && x.Allocations.Any(s => s.StartDate >= DateTime.UtcNow);

            if (query.AnalyticId > 0)
            {
                where = e => e.EndDate == null
                    && e.Allocations.Any(s => 
                        s.AnalyticId == query.AnalyticId
                        && s.StartDate >= DateTime.UtcNow);
            }

            var result = context
                .Employees
                .Where(where)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .GroupJoin(context.UserApprovers.ToList(),
                    e => e.Id,
                    w => w.EmployeeId,
                    Translate);

            if (query.ApprovalId > 0)
            {
                result = result.Where(s =>
                    s.UserApprover != null 
                    && s.UserApprover.ApproverUserId == query.ApprovalId);
            }

            return result.ToList();
        }

        public List<WorkTimeApprovalEmployee> GetByAnalytics(List<int> analyticIds, int approvalId)
        {
            Expression<Func<Employee, bool>> where = e => e.EndDate == null 
                && e.Allocations.Any(x => analyticIds.Contains(x.AnalyticId) 
                    && x.StartDate >= DateTime.UtcNow);

            var result = context
                .Employees
                .Where(where)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .GroupJoin(
                    context.UserApprovers.ToList(),
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

        public WorkTimeApprovalEmployee Translate(Employee employee, IEnumerable<UserApprover> userApprovers)
        {
            var allocation = employee.Allocations.FirstOrDefault(s => s.StartDate >= DateTime.UtcNow);
            if (parameter.AnalyticId > 0)
            {
                allocation = employee.Allocations.FirstOrDefault(s =>
                    s.AnalyticId == parameter.AnalyticId && s.StartDate >= DateTime.UtcNow);
            }

            var firstUserApprover = userApprovers.FirstOrDefault(s => s.AnalyticId == parameter.AnalyticId);

            return new WorkTimeApprovalEmployee
            {
                EmployeeId = employee.Id.ToString(),
                Name = employee.Name,
                Client = allocation?.Analytic?.ClientExternalName,
                Service = allocation?.Analytic?.Service,
                ManagerId = allocation?.Analytic?.ManagerId,
                ApprovalName = firstUserApprover?.ApproverUser?.Name,
                ClientId = allocation?.Analytic?.ClientExternalId,
                AnalyticId = allocation?.AnalyticId,
                UserApprover = firstUserApprover
            };
        }
    }
}
