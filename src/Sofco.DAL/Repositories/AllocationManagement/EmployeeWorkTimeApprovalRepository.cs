using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeWorkTimeApprovalRepository : BaseRepository<Employee>, IEmployeeWorkTimeApprovalRepository
    {
        public EmployeeWorkTimeApprovalRepository(SofcoContext context) : base(context)
        {
        }

        public List<EmployeeWorkTimeApproval> Get(WorkTimeApprovalQuery query)
        {
            Expression<Func<Employee, bool>> where = e => e.EndDate == null;

            if (query.CustomerId != Guid.Empty)
            {
                where = e => e.EndDate == null
                    && e.Allocations.Any(s => s.Analytic.ClientExternalId == query.CustomerId.ToString());
            }

            if (query.CustomerId != Guid.Empty && query.ServiceId != Guid.Empty)
            {
                where = e => e.EndDate == null
                    && e.Allocations.Any(a => a.Analytic.ClientExternalId == query.CustomerId.ToString())
                    && e.Allocations.Any(a => a.Analytic.ServiceId == query.ServiceId.ToString());
            }

            var result = context
                .Employees
                .Where(where)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .GroupJoin(context.WorkTimeApprovals.ToList(),
                    e => e.Id,
                    w => w.EmployeeId,
                    Translate);

            if (query.ApprovalId > 0)
            {
                result = result.Where(s =>
                    s.WorkTimeApproval != null 
                    && s.WorkTimeApproval.ApprovalUserId == query.ApprovalId);
            }

            return result.ToList();
        }

        public List<EmployeeWorkTimeApproval> GetByAnalytics(List<int> analyticIds, WorkTimeApprovalQuery query)
        {
            Expression<Func<Employee, bool>> where = e => e.EndDate == null && e.Allocations.Any(x => analyticIds.Contains(x.AnalyticId));

            if (query.CustomerId != Guid.Empty)
            {
                where = e => e.EndDate == null
                             && e.Allocations.Any(a => analyticIds.Contains(a.AnalyticId))
                             && e.Allocations.Any(a => a.Analytic.ClientExternalId == query.CustomerId.ToString());
            }

            if (query.CustomerId != Guid.Empty && query.ServiceId != Guid.Empty)
            {
                where = e => e.EndDate == null
                             && e.Allocations.Any(a => analyticIds.Contains(a.AnalyticId))
                             && e.Allocations.Any(a => a.Analytic.ClientExternalId == query.CustomerId.ToString())
                             && e.Allocations.Any(a => a.Analytic.ServiceId == query.ServiceId.ToString());
            }

            var result = context
                .Employees
                .Where(where)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .GroupJoin(
                    context.WorkTimeApprovals.ToList(),
                    e => e.Id,
                    w => w.EmployeeId,
                    Translate);

            if (query.ApprovalId > 0)
            {
                result = result.Where(s =>
                    s.WorkTimeApproval != null && s.WorkTimeApproval.ApprovalUserId == query.ApprovalId);
            }

            return result.ToList();
        }

        public EmployeeWorkTimeApproval Translate(Employee employee, IEnumerable<WorkTimeApproval> workTimeApprovals)
        {
            var allocation = employee.Allocations.LastOrDefault();

            var managerId = allocation?.Analytic?.ManagerId;

            var firstWorkTimeApproval = workTimeApprovals.FirstOrDefault();

            return new EmployeeWorkTimeApproval
            {
                EmployeeId = employee.Id.ToString(),
                Name = employee.Name,
                Client = allocation?.Analytic?.ClientExternalName,
                Service = allocation?.Analytic?.Service,
                ManagerId = managerId,
                ApprovalName = firstWorkTimeApproval?.ApprovalUser?.Name,
                WorkTimeApproval = firstWorkTimeApproval
            };
        }
    }
}
